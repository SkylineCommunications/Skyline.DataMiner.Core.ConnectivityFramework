using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

using Interop.SLDms;

using Skyline.DataMiner.Scripting;

using ConnectionType = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections.ConnectionType;
using DcfDebugLevel = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Debug.DcfDebugLevel;
using DcfLogType = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Logs.DcfLogType;
using DcfSaveConnectionPropertyRequest = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections.DcfSaveConnectionPropertyRequest;
using DcfSaveConnectionPropertyResult = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections.DcfSaveConnectionPropertyResult;
using DcfSaveConnectionRequest = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections.DcfSaveConnectionRequest;
using DcfSaveConnectionResult = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections.DcfSaveConnectionResult;
using DcfSaveInterfacePropertyRequest = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces.DcfSaveInterfacePropertyRequest;
using DcfSaveInterfacePropertyResult = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces.DcfSaveInterfacePropertyResult;
using SaveConnectionType = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections.SaveConnectionType;
using SyncOption = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options.SyncOption;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Helpers
{
	/// <summary>
	/// Helper class to interact with DCF.
	/// </summary>
	public class DcfHelper : IDisposable
	{
		//Indicates that even if an element is 'stopped' it will check X seconds for startup
		private int elementStartupLoopTimeInSeconds = 0;

		private Dictionary<string, HashSet<int>> currentInterfaceProperties = new Dictionary<string, HashSet<int>>();

		private Dictionary<string, HashSet<int>> currentConnectionProperties = new Dictionary<string, HashSet<int>>();

		private Dictionary<string, HashSet<int>> currentConnections = new Dictionary<string, HashSet<int>>();

		private Dictionary<string, HashSet<int>> newInterfaceProperties = new Dictionary<string, HashSet<int>>();

		private Dictionary<string, HashSet<int>> newConnectionProperties = new Dictionary<string, HashSet<int>>();

		private Dictionary<string, HashSet<int>> newConnections = new Dictionary<string, HashSet<int>>();

		private HashSet<string> unloadedElements = new HashSet<string>();

		private Options.SyncOption helperType;

		private int currentInterfacesPropertyPID = -1;

		private int currentConnectionPropertyPID = -1;

		private int currentConnectionsPID = -1;

		private int newInterfacePropertyPID = -1;

		private int newConnectionPropertyPID = -1;

		private int newConnectionsPID = -1;

		private int startupCheckPID = -1;

		private SLProtocol protocol;

		private int localDMAID;

		private int localEleID;

		private string localElementKey;

		private Dictionary<string, Collections.FastCollection<ConnectivityInterfaceProperty>> cachedInterfacePropertiesPerElement = new Dictionary<string, Collections.FastCollection<ConnectivityInterfaceProperty>>();

		private Dictionary<string, Collections.FastCollection<ConnectivityInterface>> cachedInterfacesPerElement = new Dictionary<string, Collections.FastCollection<ConnectivityInterface>>();

		private Dictionary<string, Collections.FastCollection<ConnectivityConnection>> cachedConnectionPerElement = new Dictionary<string, Collections.FastCollection<ConnectivityConnection>>();

		private Dictionary<string, Collections.FastCollection<ConnectivityConnectionProperty>> cachedConnectionPropertiesPerElement = new Dictionary<string, Collections.FastCollection<ConnectivityConnectionProperty>>();

		private HashSet<string> polledConnectionProperties = new HashSet<string>();

		private HashSet<string> polledInterfaceProperties = new HashSet<string>();

		private Debug.DcfDebugLevel debugLevel = Debug.DcfDebugLevel.None;

		private HashSet<string> checkedElements = new HashSet<string>();

		/// <summary>
		/// Initializes a new instance of the DCFHelper class. creates a DCFHelper Object for Manipulation of DCF Connections and Properties. Please use this inside of a 'using'-statement.
		/// </summary>
		/// <param name="protocol">The SLProtocol object.</param>
		/// <param name="startupCheckPID">Indicates a Parameter (saved="false") ID that will hold a mapping to indicate if a StartupCheck was already performed for a specific element (main/DVE or External).</param>
		/// <param name="removalOptions">Options to configure how the object should deal with removed connections and properties.</param>
		/// <param name="debugLevel">Indicates how much debug logging should be performed.</param>
		//[DISCodeLibrary(Version = 1)]
		private DcfHelper(SLProtocol protocol, int startupCheckPID, Options.DcfRemovalOptions removalOptions, Debug.DcfDebugLevel debugLevel = Debug.DcfDebugLevel.None)
		{
			this.debugLevel = debugLevel;
			Options.DcfMappingOptions options;
			if (removalOptions is Options.DcfMappingOptions)
			{
				options = (Options.DcfMappingOptions)removalOptions;
			}
			else if (removalOptions is Options.DcfRemovalOptionsAuto)
			{
				var tempOpt = (Options.DcfRemovalOptionsAuto)removalOptions;
				options = new Options.DcfMappingOptions();
				options.HelperType = (Options.SyncOption)tempOpt.HelperType;
				options.PIDcurrentConnectionProperties = tempOpt.PIDcurrentConnectionProperties;
				options.PIDcurrentConnections = tempOpt.PIDcurrentConnections;
				options.PIDcurrentInterfaceProperties = tempOpt.PIDcurrentInterfaceProperties;
			}
			else if (removalOptions is Options.DcfRemovalOptionsBuffer)
			{
				var tempOpt = (Options.DcfRemovalOptionsBuffer)removalOptions;
				options = new Options.DcfMappingOptions();
				options.HelperType = (Options.SyncOption)tempOpt.HelperType;
				options.PIDcurrentConnectionProperties = tempOpt.PIDcurrentConnectionProperties;
				options.PIDcurrentConnections = tempOpt.PIDcurrentConnections;
				options.PIDcurrentInterfaceProperties = tempOpt.PIDcurrentInterfaceProperties;
				options.PIDnewConnectionProperties = tempOpt.PIDnewConnectionProperties;
				options.PIDnewConnections = tempOpt.PIDnewConnections;
				options.PIDnewInterfaceProperties = tempOpt.PIDnewInterfaceProperties;
			}
			else if (removalOptions is Options.DcfRemovalOptionsManual)
			{
				//Custom
				var tempOpt = (Options.DcfRemovalOptionsManual)removalOptions;
				options = new Options.DcfMappingOptions();
				options.HelperType = (Options.SyncOption)tempOpt.HelperType;
				options.PIDcurrentConnectionProperties = tempOpt.PIDcurrentConnectionProperties;
				options.PIDcurrentConnections = tempOpt.PIDcurrentConnections;
				options.PIDcurrentInterfaceProperties = tempOpt.PIDcurrentInterfaceProperties;
			}
			else
			{
				//Default custom without mapping - shouldn't happen
				options = new Options.DcfMappingOptions();
			}

			this.protocol = protocol;
			helperType = options.HelperType;
			localDMAID = protocol.DataMinerID;
			localEleID = protocol.ElementID;
			localElementKey = localDMAID + "/" + localEleID;

			if (startupCheckPID == -1)
			{
				// StartupCheck == TRUE
				// wait on SLElement to finish starting up.

				DebugLog("QA" + protocol.QActionID + "|DCF STARTUP|Checking Startup: Main Element", LogType.Allways, LogLevel.NoLogging, Logs.DcfLogType.Setup);

				if (!IsElementStarted(this.protocol, localDMAID, localEleID))
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Startup|(ElementStartupCheck) Value {1} at DCFHelper with ERROR:{2}", protocol.QActionID, "Main Element", "Element Start Check returned False"), LogType.Error, LogLevel.NoLogging);
					throw new Exception("DCFHelper Failed to Initialize: Main Element Not Started");
				}
			}
			else if (startupCheckPID == -2)
			{
				// StartupCheck == FALSE
			}
			else
			{
				this.startupCheckPID = startupCheckPID;
				string currentStartupMap = Convert.ToString(protocol.GetParameter(startupCheckPID));

				checkedElements = new HashSet<string>(currentStartupMap.Split(';'));

				if (!checkedElements.Contains(localDMAID + "/" + localEleID))
				{
					DebugLog("QA" + protocol.QActionID + "|DCF STARTUP|Checking Startup: Main Element", LogType.Allways, LogLevel.NoLogging, Logs.DcfLogType.Setup);
					if (!IsElementStarted(this.protocol, localDMAID, localEleID))
					{
						protocol.Log(string.Format("QA{0}: |ERR: DCF Startup|(ElementStartupCheck) Value {1} at DCFHelper with ERROR:{2}", protocol.QActionID, "Main Element", "Element Start Check returned False"), LogType.Error, LogLevel.NoLogging);
					}
				}
			}

			if (options.PIDcurrentInterfaceProperties != -1)
			{
				currentInterfacesPropertyPID = options.PIDcurrentInterfaceProperties;
				PropertiesBufferToDictionary(options.PIDcurrentInterfaceProperties, currentInterfaceProperties);
			}

			if (options.PIDcurrentConnectionProperties != -1)
			{
				currentConnectionPropertyPID = options.PIDcurrentConnectionProperties;
				PropertiesBufferToDictionary(options.PIDcurrentConnectionProperties, currentConnectionProperties);
			}

			if (options.PIDcurrentConnections != -1)
			{
				currentConnectionsPID = options.PIDcurrentConnections;
				PropertiesBufferToDictionary(options.PIDcurrentConnections, currentConnections);
			}

			if (options.PIDnewInterfaceProperties != -1)
			{
				newInterfacePropertyPID = options.PIDnewInterfaceProperties;
				PropertiesBufferToDictionary(options.PIDnewInterfaceProperties, newInterfaceProperties);
			}

			if (options.PIDnewConnectionProperties != -1)
			{
				newConnectionPropertyPID = options.PIDnewConnectionProperties;
				PropertiesBufferToDictionary(options.PIDnewConnectionProperties, newConnectionProperties);
			}

			if (options.PIDnewConnections != -1)
			{
				newConnectionsPID = options.PIDnewConnections;
				PropertiesBufferToDictionary(options.PIDnewConnections, newConnections);
			}
		}

		/// <summary>
		/// Initializes a new instance of the DCFHelper class. Creates a DCFHelper Object for Manipulation of DCF Connections and Properties. Please use this inside of a 'using'-statement.
		/// </summary>
		/// <param name="protocol">The SLProtocol object.</param>
		/// <param name="startupCheck">Indicates if Element startup checks need to be forcibly performed.</param>
		/// <param name="options">Options to configure how the object should deal with removed connections and properties.</param>
		/// <param name="debugLevel">Options to configure how the object should deal with removed connections and properties</param>
		//[DISCodeLibrary(Version = 1)]
		private DcfHelper(SLProtocol protocol, bool startupCheck, Options.DcfRemovalOptions options, Debug.DcfDebugLevel debugLevel = Debug.DcfDebugLevel.None)
			: this(protocol, startupCheck ? -1 : -2, options, debugLevel)
		{
		}

		/// <summary>
		///  Initializes a new instance of the DCFHelper class that auto-clears connections and properties. Creates a DCFHelper Object for Manipulation of DCF Connections and Properties. Please use this inside of a 'using'-statement.
		/// </summary>
		/// <param name="protocol">The SLProtocol Object</param>
		/// <param name="startupCheck">Indicates if Element startup checks need to be forcibly performed.</param>
		/// <param name="options">Options holding the mapping pids needed to deal with the chosen removal strategy.</param>
		/// <param name="debugLevel">Options to configure how the object should deal with removed connections and properties</param>
		public DcfHelper(SLProtocol protocol, bool startupCheck, Options.DcfRemovalOptionsAuto options, Debug.DcfDebugLevel debugLevel = Debug.DcfDebugLevel.None)
		: this(protocol, startupCheck ? -1 : -2, options, debugLevel)
		{
		}

		/// <summary>
		///  Initializes a new instance of the DCFHelper class that buffers connections and properties for auto-removal later. Creates a DCFHelper Object for Manipulation of DCF Connections and Properties. Please use this inside of a 'using'-statement.
		/// </summary>
		/// <param name="protocol">The SLProtocol Object</param>
		/// <param name="startupCheck">Indicates if Element startup checks need to be forcibly performed.</param>
		/// <param name="options">Options holding the mapping pids needed to deal with the chosen removal strategy.</param>
		/// <param name="debugLevel">Options to configure how the object should deal with removed connections and properties</param>
		public DcfHelper(SLProtocol protocol, bool startupCheck, Options.DcfRemovalOptionsBuffer options, Debug.DcfDebugLevel debugLevel = Debug.DcfDebugLevel.None)
	: this(protocol, startupCheck ? -1 : -2, options, debugLevel)
		{
		}

		/// <summary>
		///  Initializes a new instance of the DCFHelper class that never automatically removes connections and properties. Creates a DCFHelper Object for Manipulation of DCF Connections and Properties. Please use this inside of a 'using'-statement.
		/// </summary>
		/// <param name="protocol">The SLProtocol Object</param>
		/// <param name="startupCheck">Indicates if Element startup checks need to be forcibly performed.</param>
		/// <param name="options">Options holding the mapping pids needed to deal with the chosen removal strategy.</param>
		/// <param name="debugLevel">Options to configure how the object should deal with removed connections and properties</param>
		public DcfHelper(SLProtocol protocol, bool startupCheck, Options.DcfRemovalOptionsManual options, Debug.DcfDebugLevel debugLevel = Debug.DcfDebugLevel.None)
	: this(protocol, startupCheck ? -1 : -2, options, debugLevel)
		{
		}

		/// <summary>
		///  Initializes a new instance of the DCFHelper class that auto-clears connections and properties. Creates a DCFHelper Object for Manipulation of DCF Connections and Properties. Please use this inside of a 'using'-statement.
		/// </summary>
		/// <param name="protocol">The SLProtocol Object.</param>
		/// <param name="startupCheck">A PID holding a mapping of all elements that were already checked for startup.</param>
		/// <param name="options">Options holding the mapping pids needed to deal with the chosen removal strategy.</param>
		/// <param name="debugLevel">Options to configure how the object should deal with removed connections and properties</param>
		public DcfHelper(SLProtocol protocol, int startupCheck, Options.DcfRemovalOptionsAuto options, Debug.DcfDebugLevel debugLevel = Debug.DcfDebugLevel.None)
: this(protocol, startupCheck, (Options.DcfRemovalOptions)options, debugLevel)
		{
		}

		/// <summary>
		///  Initializes a new instance of the DCFHelper class that buffers connections and properties for auto-removal later. Creates a DCFHelper Object for Manipulation of DCF Connections and Properties. Please use this inside of a 'using'-statement.
		/// </summary>
		/// <param name="protocol">The SLProtocol Object.</param>
		/// <param name="startupCheck">A PID holding a mapping of all elements that were already checked for startup.</param>
		/// <param name="options">Options holding the mapping pids needed to deal with the chosen removal strategy.</param>
		/// <param name="debugLevel">Options to configure how the object should deal with removed connections and properties</param>
		public DcfHelper(SLProtocol protocol, int startupCheck, Options.DcfRemovalOptionsBuffer options, Debug.DcfDebugLevel debugLevel = Debug.DcfDebugLevel.None)
: this(protocol, startupCheck, (Options.DcfRemovalOptions)options, debugLevel)
		{
		}

		/// <summary>
		///  Initializes a new instance of the DCFHelper class that never automatically removes connections and properties. Creates a DCFHelper Object for Manipulation of DCF Connections and Properties. Please use this inside of a 'using'-statement.
		/// </summary>
		/// <param name="protocol">The SLProtocol Object</param>
		/// <param name="startupCheck">A PID holding a mapping of all elements that were already checked for startup.</param>
		/// <param name="options">Options holding the mapping pids needed to deal with the chosen removal strategy.</param>
		/// <param name="debugLevel">Options to configure how the object should deal with removed connections and properties</param>
		public DcfHelper(SLProtocol protocol, int startupCheck, Options.DcfRemovalOptionsManual options, Debug.DcfDebugLevel debugLevel = Debug.DcfDebugLevel.None)
: this(protocol, startupCheck, (Options.DcfRemovalOptions)options, debugLevel)
		{
		}

		/// <summary>
		/// All Elements that did not succeed the startup checks.
		/// </summary>
		//[DISCodeLibrary(Version = 1)]
		public HashSet<string> UnloadedElements
		{
			get { return unloadedElements; }
			private set { unloadedElements = value; }
		}

		/// <summary>
		/// All Elements that were checked for startup
		/// </summary>
		//[DISCodeLibrary(Version = 1)]
		public HashSet<string> CheckedElements
		{
			get { return checkedElements; }
			private set { checkedElements = value; }
		}

		/// <summary>
		/// The total time in seconds that the startup checks should keep retrying. Even if the element state is 'stopped' it will keep checking.
		/// </summary>
		//[DISCodeLibrary(Version = 1)]
		public int ElementStartupLoopTimeInSeconds
		{
			get { return elementStartupLoopTimeInSeconds; }
			set { elementStartupLoopTimeInSeconds = value; }
		}

		/// <summary>
		/// Gets DCF Interfaces using DcfInterfaceFilter objects to identify a unique interface. This method caches the previously retrieved Interfaces in the background and will not retrieve the interfaces again unless the refresh bool is set to true.
		/// <para/>Returns: An array with DcfInterfaceResult Objects in the same order as the requested UIDS, if an Interface (or interfaces) was not found then this DcfInterfaceResult will be null! Be sure to check for 'null' values before using a result!
		/// </summary>
		/// <param name="refresh">Set To True if you want to force this method to perform a protocol.GetAllInterfaces and refresh it's internal Cache.</param>
		/// <param name="uids">One or more DcfInterfaceFilter objects that identify a unique interface (can be both internal, external or a mix of both)</param>
		/// <returns>An array with DcfInterfaceResult Objects in the same order as the requested UIDS, if an Interface (or interfaces) was not found then this DcfInterfaceResult will be null! Be sure to check for 'null' values before using a result!</returns>
		//[DISCodeLibrary(Version = 1)]
		public Interfaces.DcfInterfaceResult[] GetInterfaces(bool refresh, params Interfaces.DcfInterfaceFilter[] uids)
		{
			Interfaces.DcfInterfaceResult[] result = new Interfaces.DcfInterfaceResult[uids.Length];
			HashSet<string> refreshed = new HashSet<string>();
			HashSet<string> refreshedProperties = new HashSet<string>();
			for (int i = 0; i < uids.Length; i++)
			{
				ConnectivityInterface[] uidInterfaces = null;
				var uid = uids[i];
				result[i] = null;
				if (!IsElementStarted(protocol, uid.ElementKey))
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Interface|Ignoring GetInterfaceRequest: Unloaded Element:{1} ", protocol.QActionID, uid.ElementKey), LogType.Error, LogLevel.NoLogging);
					continue;
				}

				try
				{
					Collections.FastCollection<ConnectivityInterface> allInterfaces;
					if ((!cachedInterfacesPerElement.TryGetValue(uid.ElementKey, out allInterfaces) || refresh) && !refreshed.Contains(uid.ElementKey))
					{
						Dictionary<int, ConnectivityInterface> allInterfacesTmp;
						if (uid.ElementKey == "local")
						{
							allInterfacesTmp = protocol.GetConnectivityInterfaces(localDMAID, localEleID);
						}
						else
						{
							string[] elementKeyA = uid.ElementKey.Split('/');
							allInterfacesTmp = protocol.GetConnectivityInterfaces(Convert.ToInt32(elementKeyA[0]), Convert.ToInt32(elementKeyA[1]));
						}

						if (allInterfacesTmp == null)
						{
							continue;
						}

						allInterfaces = new Collections.FastCollection<ConnectivityInterface>(allInterfacesTmp.Values.ToArray());
						cachedInterfacesPerElement[uid.ElementKey] = allInterfaces;
						refreshed.Add(uid.ElementKey);
					}

					if (uid.GetAll)
					{
						uidInterfaces = allInterfaces.ToArray();
					}
					else
					{
						string uniqueKey;
						Expression<Func<ConnectivityInterface, object>> indexer;
						if (string.IsNullOrEmpty(uid.InterfaceName))
						{
							if (uid.TableKey == null)
							{
								indexer = p => Convert.ToString(p.InterfaceId);
								uniqueKey = Convert.ToString(uid.ParameterGroupID);
							}
							else if (uid.TableKey == "*")
							{
								indexer = p => Convert.ToString(p.DynamicLink);
								uniqueKey = Convert.ToString(uid.ParameterGroupID);
							}
							else
							{
								indexer = p => p.DynamicLink + "/" + p.DynamicPK;
								uniqueKey = uid.ParameterGroupID + "/" + uid.TableKey;
							}
						}
						else
						{
							if (uid.Custom)
							{
								indexer = p => p.InterfaceCustomName;
								uniqueKey = uid.InterfaceName;
							}
							else
							{
								indexer = p => p.InterfaceName;
								uniqueKey = uid.InterfaceName;
							}
						}

						allInterfaces.AddIndex(indexer);
						var allFound = allInterfaces.FindValue(indexer, uniqueKey);
						uidInterfaces = allFound.ToArray();
					}

					if (uid.PropertyFilter != null)
					{
						try
						{
							// Get All the properties
							Collections.FastCollection<ConnectivityInterfaceProperty> allProperties;
							if ((!cachedInterfacePropertiesPerElement.TryGetValue(uid.ElementKey, out allProperties) || refresh) && !refreshedProperties.Contains(uid.ElementKey))
							{
								Dictionary<int, ConnectivityInterfaceProperty> allPropsTmp = new Dictionary<int, ConnectivityInterfaceProperty>();
								foreach (var intf in allInterfaces)
								{
									intf.InterfaceProperties.ToList().ForEach(x => allPropsTmp.Add(x.Key, x.Value));
								}

								allProperties = new Collections.FastCollection<ConnectivityInterfaceProperty>(allPropsTmp.Values.ToArray());

								cachedInterfacePropertiesPerElement[uid.ElementKey] = allProperties;
								refreshedProperties.Add(uid.ElementKey);
							}

							string uniquePropertyKey = string.Empty;
							Expression<Func<ConnectivityInterfaceProperty, object>> propertyIndexer = null;
							Func<ConnectivityInterfaceProperty, object> indexerSearch = null;

							Filters.DcfPropertyFilter propFilter = uid.PropertyFilter;
							if (propFilter.ID != -1)
							{
								indexerSearch = p => p.InterfacePropertyId;
								uniquePropertyKey = Convert.ToString(propFilter.ID);
							}
							else
							{
								if (!string.IsNullOrEmpty(propFilter.Name))
								{
									indexerSearch = p => p.InterfacePropertyName;
									uniquePropertyKey = propFilter.Name;
								}

								if (!string.IsNullOrEmpty(propFilter.Type))
								{
									if (indexerSearch == null)
									{
										indexerSearch = p => p.InterfacePropertyType;
										uniquePropertyKey = propFilter.Type;
									}
									else
									{
										var temp = indexerSearch;
										indexerSearch = p => temp(p) + "/" + p.InterfacePropertyType;
										uniquePropertyKey = uniquePropertyKey + "/" + propFilter.Type;
									}
								}

								if (!string.IsNullOrEmpty(propFilter.Value))
								{
									if (indexerSearch == null)
									{
										indexerSearch = p => p.InterfacePropertyValue;
										uniquePropertyKey = propFilter.Value;
									}
									else
									{
										var temp = indexerSearch;
										indexerSearch = p => temp(p) + "/" + p.InterfacePropertyValue;
										uniquePropertyKey = uniquePropertyKey + "/" + propFilter.Value;
									}
								}
							}

							if (indexerSearch != null)
							{
								propertyIndexer = p => indexerSearch(p);

								allProperties.AddIndex(propertyIndexer);
								var foundProperties = allProperties.FindValue(propertyIndexer, uniquePropertyKey);

								// make a list
								HashSet<string> foundInterfaces = new HashSet<string>(foundProperties.Select(p => p.Interface.ElementKey + "/" + p.Interface.InterfaceId));

								// Filter Current Results with the found Property Interfaces
								uidInterfaces = uidInterfaces.Where(p => foundInterfaces.Contains(p.ElementKey + "/" + p.InterfaceId)).ToArray();
							}
						}
						catch (Exception e)
						{
							protocol.Log(string.Format("QA{0}:|ERR: DCF Interface|(Exception) Value {1} at GetInterfaces - By Property with Exception:{2}", protocol.QActionID, uid, e.ToString()), LogType.Error, LogLevel.NoLogging);
						}
					}

					if (uid is Interfaces.DcfInterfaceFilterSingle)
					{
						Interfaces.DcfInterfaceResultSingle newResult = new Interfaces.DcfInterfaceResultSingle(uid, uidInterfaces);
						result[i] = newResult;
					}
					else
					{
						Interfaces.DcfInterfaceResultMulti newResult = new Interfaces.DcfInterfaceResultMulti(uid, uidInterfaces);
						result[i] = newResult;
					}
				}
				catch (Exception e)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Interface|(Exception) Value {1} at GetInterfaces with Exception:{2}", protocol.QActionID, uid, e.ToString()), LogType.Error, LogLevel.NoLogging);
				}
			}

			return result;
		}

		/// <summary>
		/// Gets DCF Interfaces using DcfInterfaceFilter structs to identify a unique interface. This method caches the previously retrieved Interfaces in the background and will not retrieve the interfaces again unless the refresh bool is set to true.
		/// <para/>Returns: An array with DCFDynamicLinkResult Objects in the same order as the requested UIDS, if an Interface (or interfaces) was not found then this DcfInterfaceResult will be null!
		/// </summary>
		/// <param name="uids">One or more DcfInterfaceFilter structs that identify a unique interface (can be both internal, external or a mix of both)</param>
		/// <returns>An array with DCFDynamicLinkResult Objects in the same order as the requested UIDS, if an Interface (or interfaces) was not found then this DcfInterfaceResult will be null!</returns>
		//[DISCodeLibrary(Version = 1)]
		public Interfaces.DcfInterfaceResult[] GetInterfaces(params Interfaces.DcfInterfaceFilter[] uids)
		{
			return GetInterfaces(false, uids);
		}

		/// <summary>
		/// Gets a single ConnectivityInterface based on a single filter.
		/// <para/>Returns: A single ConnectivityInterface object, returns Null if not found.
		/// </summary>
		/// <param name="filter">Filter used to find the interface</param>
		/// <param name="refresh">Indicates if it should forcibly refresh the internal cache of interfaces</param>
		/// <returns>A single ConnectivityInterface object, returns Null if not found.</returns>
		//[DISCodeLibrary(Version = 1)]
		public ConnectivityInterface GetInterface(Interfaces.DcfInterfaceFilterSingle filter, bool refresh = false)
		{
			var result = GetInterfaces(refresh, filter);
			if (result[0] == null)
			{
				return null;
			}
			else
			{
				return ((Interfaces.DcfInterfaceResultSingle)GetInterfaces(refresh, filter)[0]).DCFInterface;
			}
		}

		/// <summary>
		/// Gets a collection of ConnectivityInterfaces based on a filter.
		/// <para/>Returns: Multiple ConnectivityInterfaces matching the filter. Returns Null if none where found.
		/// </summary>
		/// <param name="filter">Filter used to find the interfaces</param>
		/// <param name="refresh">Indicates if it should forcibly refresh the internal cache of interfaces</param>
		/// <returns>Multiple ConnectivityInterfaces matching the filter. Returns Null if none where found.</returns>
		//[DISCodeLibrary(Version = 1)]
		public ConnectivityInterface[] GetInterfaces(Interfaces.DcfInterfaceFilterMulti filter, bool refresh = false)
		{
			var result = GetInterfaces(refresh, filter);
			if (result[0] == null)
			{
				return null;
			}
			else
			{
				return ((Interfaces.DcfInterfaceResultMulti)GetInterfaces(refresh, filter)[0]).DcfInterfaces;
			}
		}

		/// <summary>
		/// GetConnections allows the retrieval of connections based on DCFConnectionFilters using an internal cache.
		/// <para/>Returns: An array of DCFConnectionResult objects in the same order as the requests. If one of the filter found nothing, that resultObject will be Null. Make sure to check for this.
		/// </summary>
		/// <param name="forceRefresh">Forces the refresh of the internal cache</param>
		/// <param name="requests">One or more DCFConnectionFilter objects</param>
		/// <returns>An array of DCFConnectionResult objects in the same order as the requests. If one of the filter found nothing, that resultObject will be Null. Make sure to check for this.</returns>
		//[DISCodeLibrary(Version = 1)]
		public Connections.DcfConnectionResult[] GetConnections(bool forceRefresh, params Connections.DcfConnectionFilter[] requests)
		{
			protocol.Log("QA" + protocol.QActionID + "|TEMP|1.1", LogType.Error, LogLevel.NoLogging);
			Connections.DcfConnectionResult[] result = new Connections.DcfConnectionResult[requests.Length];
			for (int i = 0; i < requests.Length; i++)
			{
				//Default Values:
				var request = requests[i];
				result[i] = null;
				int requestDMAId;
				int requestEleId;
				string requestElementKey;

				if (request.DestinationFilter != null)
				{
					request.DestinationInterface = GetInterface(request.DestinationFilter);
					if (request.DestinationInterface == null)
					{
						protocol.Log("QA" + protocol.QActionID + "|ERR|QA" + protocol.QActionID + "|DCF Connection|GetConnections: Destination Interface Not Found for Request:" + i, LogType.Error, LogLevel.NoLogging);
						//DebugLog("QA" + protocol.QActionID + "|DCF Connection|GetConnections: Source Interface Not Found",LogType.Error,LogLevel.NoLogging,DcfLogType.Info);
						continue;
					}
				}

				if (request.SourceFilter != null)
				{
					request.SourceInterface = GetInterface(request.SourceFilter);
					if (request.SourceInterface == null)
					{
						protocol.Log("QA" + protocol.QActionID + "|ERR|QA" + protocol.QActionID + "|DCF Connection|GetConnections: Source Interface Not Found for Request:" + i, LogType.Error, LogLevel.NoLogging);
						continue;
					}
				}

				if (string.IsNullOrEmpty(request.ElementKey))
				{
					if (request.SourceInterface != null)
					{
						requestDMAId = request.SourceInterface.DataMinerId;
						requestEleId = request.SourceInterface.ElementId;
						requestElementKey = request.SourceInterface.ElementKey;
					}
					else
					{
						requestDMAId = localDMAID;
						requestEleId = localEleID;
						requestElementKey = localElementKey;
					}
				}
				else
				{
					if (request.ElementKey == "local")
					{
						requestElementKey = localElementKey;
						requestDMAId = localDMAID;
						requestEleId = localEleID;
					}
					else
					{
						requestElementKey = request.ElementKey;
						string[] splitKey = requestElementKey.Split('/');
						if (splitKey.Length > 1)
						{
							requestDMAId = Convert.ToInt32(splitKey[0]);
							requestEleId = Convert.ToInt32(splitKey[1]);
						}
						else
						{
							protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Ignoring GetConnections: Invalid Format ElementKey:{1} ", protocol.QActionID, requestElementKey), LogType.Error, LogLevel.NoLogging);
							continue;
						}
					}
				}
				protocol.Log("QA" + protocol.QActionID + "|TEMP|1.2", LogType.Error, LogLevel.NoLogging);
				string uniqueKey = null;
				Expression<Func<ConnectivityConnection, object>> indexer = null;

				if (!IsElementStarted(protocol, requestElementKey))
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Ignoring GetConnections: Unloaded Element:{1} ", protocol.QActionID, requestElementKey), LogType.Error, LogLevel.NoLogging);
					continue;
				}
				protocol.Log("QA" + protocol.QActionID + "|TEMP|1.3", LogType.Error, LogLevel.NoLogging);
				string internalExternal;
				switch (request.Type)
				{
					case Connections.ConnectionType.Internal:
						internalExternal = "I";
						break;

					case Connections.ConnectionType.External:
						internalExternal = "E";
						break;

					case Connections.ConnectionType.Both:
						internalExternal = "B";
						break;

					default:
						internalExternal = "B";
						break;
				}

				// ConnectionID
				if (request.ConnectionID != -1)
				{
					indexer = p => InternalExternalChar(p, request.Type) + "_" + Convert.ToString(p.ConnectionId);
					uniqueKey = internalExternal + "_" + Convert.ToString(request.ConnectionID);
				}
				// ConnectionName
				if (request.ConnectionName != null)
				{
					indexer = p => InternalExternalChar(p, request.Type) + "_" + Convert.ToString(p.ConnectionName);
					uniqueKey = internalExternal + "_" + Convert.ToString(request.ConnectionName);
				}

				if (request.DestinationInterface != null)
				{
					if (request.SourceInterface != null)
					{
						// Source + Destination
						indexer = p => InternalExternalChar(p, request.Type) + "_" + p.SourceInterfaceId + "/" + p.DestinationDMAId + "/" + p.DestinationEId + "/" + p.DestinationInterfaceId;
						uniqueKey = internalExternal + "_" + request.SourceInterface.InterfaceId + "/" + request.DestinationInterface.ElementKey + "/" + request.DestinationInterface.InterfaceId;
					}
					else
					{
						// only Destination
						indexer = p => InternalExternalChar(p, request.Type) + "_" + p.DestinationDMAId + "/" + p.DestinationEId + "/" + p.DestinationInterfaceId;
						uniqueKey = internalExternal + "_" + request.DestinationInterface.DataMinerId + "/" + request.DestinationInterface.ElementId + "/" + request.DestinationInterface.InterfaceId;
					}
				}
				else if (request.SourceInterface != null)
				{
					// only Source
					indexer = p => InternalExternalChar(p, request.Type) + "_" + p.SourceInterfaceId;
					uniqueKey = internalExternal + "_" + Convert.ToString(request.SourceInterface.InterfaceId);
				}
				else
				{
					//Default to getting all the connections from the element (external, internal or both) if no filter is provided.
					indexer = p => InternalExternalChar(p, request.Type);
					uniqueKey = internalExternal;
				}

				protocol.Log("QA" + protocol.QActionID + "|TEMP|1.4", LogType.Error, LogLevel.NoLogging);
				if (!cachedConnectionPerElement.ContainsKey(requestElementKey) || forceRefresh)
				{
					var newPolledConnections = protocol.GetConnectivityConnections(requestDMAId, requestEleId);
					if (newPolledConnections == null)
					{
						DebugLog("QA" + protocol.QActionID + "DCF Connection|GetConnectivityConnections returned a Null for Element:" + requestElementKey + " Either there was No Response, SLNet was not available, there were NO connections, or there was an Exception in the DataMiner DCF API code", LogType.Allways, LogLevel.NoLogging, DcfLogType.Info);
						result[i] = null;
						continue;
					}

					cachedConnectionPerElement[requestElementKey] = new Collections.FastCollection<ConnectivityConnection>(newPolledConnections.Values.ToList());
				}
				protocol.Log("QA" + protocol.QActionID + "|TEMP|1.5", LogType.Error, LogLevel.NoLogging);
				Collections.FastCollection<ConnectivityConnection> elementConnections = cachedConnectionPerElement[requestElementKey];
				if (indexer != null && uniqueKey != null)
				{
					var foundConnections = elementConnections.FindValue(indexer, uniqueKey).ToArray();
					if (foundConnections != null && foundConnections.Length > 0)
					{
						result[i] = new Connections.DcfConnectionResult(request, elementConnections.FindValue(indexer, uniqueKey).ToArray());
					}
					else
					{
						result[i] = null;
					}
				}
				else
				{
					if (elementConnections != null && elementConnections.Count() > 0)
					{
						result[i] = new Connections.DcfConnectionResult(request, elementConnections.ToArray());
					}
					else
					{
						result[i] = null;
					}
				}

				//Now filter on properties

				if (request.PropertyFilter != null && result[i] != null)
				{
					//ToDo write code to filter on connectionProperties check the GetInterfaces to see an example
				}
			}
			protocol.Log("QA" + protocol.QActionID + "|TEMP|1.result", LogType.Error, LogLevel.NoLogging);
			return result;
		}

		/// <summary>
		/// GetConnections allows the retrieval of connections based on DCFConnectionFilters using an internal cache.
		/// <para/>Returns: An array of DCFConnectionResult objects in the same order as the requests. If one of the filter found nothing, that resultObject will be Null. Make sure to check for this.
		/// </summary>
		/// <param name="requests">One or more DCFConnectionFilter objects</param>
		/// <returns>An array of DCFConnectionResult objects in the same order as the requests. If one of the filter found nothing, that resultObject will be Null. Make sure to check for this.</returns>
		//[DISCodeLibrary(Version = 1)]
		public Connections.DcfConnectionResult[] GetConnections(params Connections.DcfConnectionFilter[] requests)
		{
			return GetConnections(false, requests);
		}

		/// <summary>
		/// GetConnections allows the retrieval of connections based on DCFConnectionFilters using an internal cache.
		/// <para/>Returns: An array of ConnectivityConnection objects in the same order as the requests. If the filter found nothing, the returned array will be Null. Make sure to check for this.
		/// </summary>
		/// <param name="request">One DCFConnectionFilter objects where you expect one or more ConnectivityConnections as a result.</param>
		/// <returns>An array of ConnectivityConnection objects in the same order as the requests. If the filter found nothing, the returned array will be Null. Make sure to check for this.</returns>
		//[DISCodeLibrary(Version = 1)]
		public ConnectivityConnection[] GetConnections(Connections.DcfConnectionFilter request)
		{
			var connectionResult = GetConnections(false, request)[0];
			return connectionResult != null ? connectionResult.Connections : null;
		}

		/// <summary>
		/// GetConnections allows the retrieval of connections based on DCFConnectionFilters using an internal cache.
		/// <para/>Returns: A single ConnectivityConnection object that matches your request. Returns Null if no connection was found matching the filter.
		/// </summary>
		/// <param name="request">One DCFConnectionFilter objects where you expect one ConnectivityConnection object as a result.</param>
		/// <returns>A single ConnectivityConnection object that matches your request. Returns Null if no connection was found matching the filter.</returns>
		//[DISCodeLibrary(Version = 1)]
		public ConnectivityConnection GetConnection(Connections.DcfConnectionFilter request)
		{
			protocol.Log("QA" + protocol.QActionID + "|TEMP|1", LogType.Error, LogLevel.NoLogging);
			var connectionResult = GetConnections(false, request)[0];
			protocol.Log("QA" + protocol.QActionID + "|TEMP|2", LogType.Error, LogLevel.NoLogging);
			return connectionResult != null ? connectionResult.Connections[0] : null;
		}

		/// <summary>
		/// This method is used to save both internal and external connections.
		/// <para/>Returns: One or more DCFSaveConnectionResults in the same order as the DCFSaveConnectionRequests. If a Connection fails to get created or if you use Async then the connections inside the DCFSaveConnectionResult will be null.
		/// </summary>
		/// <param name="forceRefresh">Indicates if the cache of Connections needs to be refreshed (performs a protocol.GetConnectivityConnections() in the background)</param>
		/// <param name="requests">One or more DCFSaveConnectionRequest objects that define the connection you wish to save</param>
		/// <returns>One or more DCFSaveConnectionResults in the same order as the DCFSaveConnectionRequests. If a Connection fails to get created or if you use Async then the connections inside the DCFSaveConnectionResult will be null.</returns>
		//[DISCodeLibrary(Version = 1)]
		public Connections.DcfSaveConnectionResult[] SaveConnections(bool forceRefresh, params Connections.DcfSaveConnectionRequest[] requests)
		{
			Connections.DcfSaveConnectionResult[] result = new Connections.DcfSaveConnectionResult[requests.Length];
			for (int i = 0; i < requests.Length; i++)
			{
				Connections.DcfSaveConnectionRequest currentRequest = requests[i];
				// Make sure, if properties are requested. The save is sync.
				if (currentRequest.PropertyRequests != null)
				{
					if (currentRequest.PropertyRequests.Count > 0)
					{
						currentRequest.Async = false;
					}
				}

				result[i] = new Connections.DcfSaveConnectionResult(null, null, false, true, null);
				bool updated = true;
				if (currentRequest == null)
				{
					continue;
				}

				if (currentRequest.Source == null || currentRequest.Destination == null)
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|ConnectionRequest Had empty Source or Destination. The Requested Interfaces might not exist.", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
					continue;
				}

				try
				{
					if (currentRequest.CustomName == null)
					{
						currentRequest.CustomName = currentRequest.Source.InterfaceName + "->" + currentRequest.Destination.InterfaceName;
					}

					string sourceElementKey = currentRequest.Source.ElementKey;
					if (!IsElementStarted(protocol, sourceElementKey))
					{
						protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Ignoring ConnectionRequest Unloaded Source Element:{1} ", protocol.QActionID, sourceElementKey), LogType.Error, LogLevel.NoLogging);
						continue;
					}

					string destinElementKey = currentRequest.Destination.ElementKey;
					if (!IsElementStarted(protocol, destinElementKey))
					{
						protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Ignoring ConnectionRequest Unloaded Destination Element:{1} ", protocol.QActionID, destinElementKey), LogType.Error, LogLevel.NoLogging);
						continue;
					}

					bool internalConnection = sourceElementKey == destinElementKey;

					if (!cachedConnectionPerElement.ContainsKey(sourceElementKey) || forceRefresh)
					{
						var newPolledConnections = protocol.GetConnectivityConnections(currentRequest.Source.DataMinerId, currentRequest.Source.ElementId);
						if (newPolledConnections == null)
						{
							protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|GetConnectivityConnections returned a Null for Element:" + currentRequest.Source.DataMinerId + "/" + currentRequest.Source.ElementId + " Either there was No Response, SLNet was not available, or there was an Exception in the DataMiner DCF API code.", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
							continue;
						}

						cachedConnectionPerElement[sourceElementKey] = new Collections.FastCollection<ConnectivityConnection>(newPolledConnections.Values.ToList());
					}

					string uniqueKey;
					Collections.FastCollection<ConnectivityConnection> elementConnections = cachedConnectionPerElement[sourceElementKey];
					Expression<Func<ConnectivityConnection, object>> indexer;
					switch (currentRequest.ConnectionType)
					{
						case SaveConnectionType.Unique_Name:
							indexer = p => InternalExternalChar(p) + "_" + p.ConnectionName;
							uniqueKey = InternalExternalChar(currentRequest) + "_" + currentRequest.CustomName;
							break;

						case SaveConnectionType.Unique_Destination:
							indexer = p => InternalExternalChar(p) + "_" + p.DestinationDMAId + "/" + p.DestinationEId + "/" + p.DestinationInterfaceId;
							uniqueKey = InternalExternalChar(currentRequest) + "_" + currentRequest.Destination.DataMinerId + "/" + currentRequest.Destination.ElementId + "/" + currentRequest.Destination.InterfaceId;
							break;

						case SaveConnectionType.Unique_Source:
							indexer = p => InternalExternalChar(p) + "_" + p.SourceInterfaceId;
							uniqueKey = InternalExternalChar(currentRequest) + "_" + Convert.ToString(currentRequest.Source.InterfaceId);
							break;

						case SaveConnectionType.Unique_SourceAndDestination:
							indexer = p => InternalExternalChar(p) + "_" + p.SourceInterfaceId + "/" + p.DestinationDMAId + "/" + p.DestinationEId + "/" + p.DestinationInterfaceId;
							uniqueKey = InternalExternalChar(currentRequest) + "_" + currentRequest.Source.InterfaceId + "/" + currentRequest.Destination.ElementKey + "/" + currentRequest.Destination.InterfaceId;
							break;

						default:
							indexer = p => InternalExternalChar(p) + "_" + p.ConnectionName;
							uniqueKey = InternalExternalChar(currentRequest) + "_" + currentRequest.CustomName;
							break;
					}

					elementConnections.AddIndex(indexer);
					// Find Original Connection
					ConnectivityConnection matchingConnection = elementConnections.FindValue(indexer, uniqueKey).FirstOrDefault();
					ConnectivityConnection newDestinationConnection = null;
					int sourceId = -1;
					int destinationId = -1;
					if (matchingConnection == null)
					{
						// Add a new Connection
						if (internalConnection)
						{
							DebugLog("QA" + protocol.QActionID + "|DCF Connection|Adding Internal Connection:" + currentRequest.CustomName + " | With Connection Filter: " + currentRequest.ConnectionFilter + " | on Element:" + currentRequest.Source.ElementKey, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

							// add an internal connection
							if (!currentRequest.Async)
							{
								if (!currentRequest.Source.AddConnection(currentRequest.CustomName, currentRequest.CustomName, currentRequest.Destination, currentRequest.ConnectionFilter, false, out matchingConnection, out newDestinationConnection, 420000))
								{
									protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Adding Internal DCF Connection -sync:{1} on element {2} Timed-Out after 7 minutes or returned false. Connection may not have been added", protocol.QActionID, currentRequest.CustomName, sourceElementKey), LogType.Error, LogLevel.NoLogging);
								}

								if (matchingConnection != null) sourceId = matchingConnection.ConnectionId;
								if (newDestinationConnection != null) destinationId = newDestinationConnection.ConnectionId;
							}
							else
							{
								if (!currentRequest.Source.AddConnection(currentRequest.CustomName, currentRequest.CustomName, currentRequest.Destination, currentRequest.ConnectionFilter, false, out sourceId, out destinationId))
								{
									protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Adding Internal DCF Connection -async:{1} on element {2} returned false. Connection may not have been added", protocol.QActionID, currentRequest.CustomName, sourceElementKey), LogType.Error, LogLevel.NoLogging);
								}
							}
						}
						else
						{
							DebugLog("QA" + protocol.QActionID + "|DCF Connection|Adding External Connection:" + currentRequest.CustomName + " | With Connection Filter: " + currentRequest.ConnectionFilter + " | from Element:" + currentRequest.Source.ElementKey + " To Element:" + currentRequest.Destination.ElementKey, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

							// add an external connection
							if (!currentRequest.Async)
							{
								if (!currentRequest.Source.AddConnection(currentRequest.CustomName, currentRequest.CustomName + " -RETURN", currentRequest.Destination, currentRequest.ConnectionFilter, currentRequest.CreateExternalReturn, out matchingConnection, out newDestinationConnection, 420000))
								{
									protocol.Log(string.Format("QA{0}:|ERR: DCF Connection|Adding External DCF Connection:{1} from element {2} to element {3} Timed-Out after 7 minutes or returned false. Connection may not have been added", protocol.QActionID, currentRequest.CustomName, sourceElementKey, currentRequest.Destination), LogType.Error, LogLevel.NoLogging);
								}
								if (matchingConnection != null) sourceId = matchingConnection.ConnectionId;
								if (newDestinationConnection != null) destinationId = newDestinationConnection.ConnectionId;
							}
							else
							{
								if (!currentRequest.Source.AddConnection(currentRequest.CustomName, currentRequest.CustomName + " -RETURN", currentRequest.Destination, currentRequest.ConnectionFilter, currentRequest.CreateExternalReturn, out sourceId, out destinationId))
								{
									protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Adding Internal DCF Connection -async:{1} on element {2} returned false. Connection may not have been added", protocol.QActionID, currentRequest.CustomName, sourceElementKey), LogType.Error, LogLevel.NoLogging);
								}
							}
						}
					}
					else
					{
						// Update the Connection
						// Check if Update is Necessary
						if (
							matchingConnection.ConnectionName == currentRequest.CustomName
							&& matchingConnection.SourceDataMinerId + "/" + matchingConnection.SourceElementId == currentRequest.Source.ElementKey
							&& matchingConnection.SourceInterfaceId == currentRequest.Source.InterfaceId
							&& matchingConnection.DestinationDMAId + "/" + matchingConnection.DestinationEId == currentRequest.Destination.ElementKey
							&& matchingConnection.DestinationInterfaceId == currentRequest.Destination.InterfaceId
							&& matchingConnection.ConnectionFilter == currentRequest.ConnectionFilter)
						{
							// NO UPDATE NECESSARY
							updated = false;

							if (internalConnection)
							{
								DebugLog("QA" + protocol.QActionID + "|DCF Connection (" + matchingConnection.ConnectionId + ") |Not Updating Internal Connection (ID:" + matchingConnection.ConnectionId + ") To:" + currentRequest.CustomName + " on Element:" + currentRequest.Source.ElementKey + "-- No Change Detected", LogType.Allways, LogLevel.NoLogging, DcfLogType.Same);
							}
							else
							{
								DebugLog("QA" + protocol.QActionID + "|DCF Connection (" + matchingConnection.ConnectionId + ") |Not Updating External Connection (ID:" + matchingConnection.ConnectionId + ") To:" + currentRequest.CustomName + " from Element:" + currentRequest.Source.ElementKey + " To Element:" + currentRequest.Destination.ElementKey + "-- No Change Detected", LogType.Allways, LogLevel.NoLogging, DcfLogType.Same);
							}
						}
						else
						{
							// UPDATE NECESSARY
							if (internalConnection)
							{
								DebugLog("QA" + protocol.QActionID + "|DCF Connection (" + matchingConnection.ConnectionId + ") |Updating Internal Connection (ID:" + matchingConnection.ConnectionId + ") To:" + currentRequest.CustomName + " | With Connection Filter: " + currentRequest.ConnectionFilter + " | on Element:" + currentRequest.Source.ElementKey, LogType.Allways, LogLevel.NoLogging, Logs.DcfLogType.Change);
								if (!matchingConnection.Update(currentRequest.CustomName, currentRequest.Source.InterfaceId, currentRequest.CustomName, currentRequest.Destination.DataMinerId, currentRequest.Destination.ElementId, currentRequest.Destination.InterfaceId, currentRequest.ConnectionFilter, false, out newDestinationConnection, 420000))
								{
									protocol.Log(string.Format("QA{0}:|ERR: DCF Connection (" + matchingConnection.ConnectionId + ") | Updating Internal DCF Connection:{1} on element {2} Timed-Out after 7 minutes or returned false. Connection may not have been updated", protocol.QActionID, currentRequest.CustomName, sourceElementKey), LogType.Error, LogLevel.NoLogging);
								}
							}
							else
							{
								DebugLog("QA" + protocol.QActionID + "|DCF Connection (" + matchingConnection.ConnectionId + ") |Updating External Connection (ID:" + matchingConnection.ConnectionId + ") To:" + currentRequest.CustomName + " | With Connection Filter: " + currentRequest.ConnectionFilter + " | from Element:" + currentRequest.Source.ElementKey + " To Element:" + currentRequest.Destination.ElementKey, LogType.Allways, LogLevel.NoLogging, DcfLogType.Same);
								if (!matchingConnection.Update(currentRequest.CustomName, currentRequest.Source.InterfaceId, currentRequest.CustomName + " -RETURN", currentRequest.Destination.DataMinerId, currentRequest.Destination.ElementId, currentRequest.Destination.InterfaceId, currentRequest.ConnectionFilter, currentRequest.CreateExternalReturn, out newDestinationConnection, 420000))
								{
									protocol.Log(string.Format("QA{0}:|ERR: DCF Connection (" + matchingConnection.ConnectionId + ") | Updating External DCF Connection:{1} from element {2} to element {3} Timed-Out after 7 minutes or returned false. Connection may not have been updated", protocol.QActionID, currentRequest.CustomName, sourceElementKey, currentRequest.Destination.ElementKey), LogType.Error, LogLevel.NoLogging);
								}
							}
						}
						if (matchingConnection != null) sourceId = matchingConnection.ConnectionId;
						if (newDestinationConnection != null) destinationId = newDestinationConnection.ConnectionId;
					}
					protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|4", LogType.Error, LogLevel.NoLogging);
					string inpEleKye = CreateElementKey(currentRequest.Source.DataMinerId, currentRequest.Source.ElementId);
					if (currentRequest.FixedConnection)
					{// Indicating fixed connections with negative values
						if (sourceId != -1)
						{
							AddToPropertyDictionary(newConnections, inpEleKye, sourceId * -1);
						}
					}
					else
					{
						protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|5", LogType.Error, LogLevel.NoLogging);
						AddToPropertyDictionary(newConnections, inpEleKye, sourceId);
						protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|6", LogType.Error, LogLevel.NoLogging);
					}
					protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|6.1", LogType.Error, LogLevel.NoLogging);
					DcfSaveConnectionPropertyResult[] propertyResults = null;
					if (currentRequest.PropertyRequests != null && currentRequest.PropertyRequests.Count() > 0)
					{
						protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|6.2", LogType.Error, LogLevel.NoLogging);
						if (!currentRequest.Async)
						{
							protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|7", LogType.Error, LogLevel.NoLogging);
							propertyResults = SaveConnectionProperties(matchingConnection, currentRequest.PropertyRequests.ToArray());
						}
						else
						{
							protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|6.2.1", LogType.Error, LogLevel.NoLogging);
							if (matchingConnection == null) protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|MatchCon null", LogType.Error, LogLevel.NoLogging);
							protocol.Log(string.Format("QA{0}:|ERR: DCF Connection (" + sourceId + ") | Saving Properties on an Async created connection is not supported. Please use synchronous SaveConnections", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
							protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|6.2.1.1", LogType.Error, LogLevel.NoLogging);
						}
					}
					protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|6.3", LogType.Error, LogLevel.NoLogging);
					if (matchingConnection == null && newDestinationConnection == null)
					{
						protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|8", LogType.Error, LogLevel.NoLogging);
						result[i] = new DcfSaveConnectionResult(sourceId, destinationId, internalConnection, updated, propertyResults);
						protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|9", LogType.Error, LogLevel.NoLogging);
					}
					else
					{
						protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|10", LogType.Error, LogLevel.NoLogging);
						result[i] = new DcfSaveConnectionResult(matchingConnection, newDestinationConnection, internalConnection, updated, propertyResults);
						protocol.Log("QA" + protocol.QActionID + "|TEMPORARY|11", LogType.Error, LogLevel.NoLogging);
					}
				}
				catch (Exception e)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Connection| Exception in SaveConnections for connectionRequest:{1}  with exception:{2}", protocol.QActionID, currentRequest.CustomName, e.ToString()), LogType.Error, LogLevel.NoLogging);
				}
			}

			return result;
		}

		/// <summary>
		/// This method is used to save both internal and external connections.
		/// <para/>Returns: One or more DCFSaveConnectionResults in the same order as the DCFSaveConnectionRequests. If a Connection fails to get created or if you use Async then the connections inside the DCFSaveConnectionResult will be null.
		/// </summary>
		/// <param name="requests">One or more DCFSaveConnectionRequest objects that define the connection you wish to save</param>
		/// <returns>One or more DCFSaveConnectionResults in the same order as the DCFSaveConnectionRequests. If a Connection fails to get created or if you use Async then the connections inside the DCFSaveConnectionResult will be null.</returns>
		//[DISCodeLibrary(Version = 1)]
		public DcfSaveConnectionResult[] SaveConnections(params DcfSaveConnectionRequest[] requests)
		{
			return SaveConnections(false, requests);
		}

		/// <summary>
		/// Removes all Connections with the given Name.
		/// <para/>Returns: False, if the Remove Failed. (Note, this will be logged as an ERR in protocol logging).
		/// </summary>
		/// <param name="input">ConnectivityInterface object containing the connection</param>
		/// <param name="bothConnections">Indicates of both connections (for external connections) must be removed</param>
		/// <param name="force">Indicates if the removal should be Forced and not checked if it's a legal removal</param>
		/// <param name="connectionNames">Connection Names for Connections that need to be deleted.</param>
		//[DISCodeLibrary(Version = 1)]
		public bool RemoveConnections(ConnectivityInterface input, bool bothConnections, bool force, params string[] connectionNames)
		{
			try
			{
				bool nullInputDetected = false;
				if (input == null)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Connection| Removing(A) DCF Connections ConnectivityInterface input was Null", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
					nullInputDetected = true;
				}

				if (nullInputDetected)
				{
					return false;
				}

				List<int> connectionsToDelete = new List<int>();
				for (int u = 0; u < connectionNames.Length; u++)
				{
					ConnectivityConnection con = input.GetConnectionByName(connectionNames[u]);
					if (con == null)
					{
						continue;
					}

					int id = con.ConnectionId;
					connectionsToDelete.Add(id);
				}

				return RemoveConnections(input, bothConnections, force, connectionsToDelete.ToArray());
			}
			catch (Exception e)
			{
				protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|(Exception) at RemoveConnections with Exception:{1}", protocol.QActionID, e.ToString()), LogType.Error, LogLevel.NoLogging);
			}

			return false;
		}

		/// <summary>
		/// Removed all Connections with the given IDs For the provided Interface.
		/// <para/>Returns: False, if the Remove Failed. (Note, this will be logged as an ERR in protocol logging).
		/// </summary>
		/// <param name="input">ConnectivityInterface object containing the connection</param>
		/// <param name="bothConnections">Indicates of both connections (for external connections) must be removed</param>
		/// <param name="force">Indicates if the removal should be Forced and not checked if it's a legal removal</param>
		/// <param name="connectionIDs">All Connection IDs for Connections that need to be deleted</param>
		//[DISCodeLibrary(Version = 1)]
		public bool RemoveConnections(ConnectivityInterface input, bool bothConnections, bool force, params int[] connectionIDs)
		{
			try
			{
				bool nullInputDetected = false;
				if (input == null)
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Removing DCF(B) Connections ConnectivityInterface input was Null", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
					nullInputDetected = true;
				}

				if (nullInputDetected)
				{
					return false;
				}

				if (currentConnectionsPID == -1)
				{
					protocol.Log("QA" + protocol.QActionID + "|ERR: DCF Connection|DCFHelper Error: Using RemoveConnections requires the CurrentConnectionsPID to be defined! Please change the Options Objects to include this PID", LogType.Error, LogLevel.NoLogging);
					return false;
				}

				bool finalResult = true;
				string eleKey = CreateElementKey(input.DataMinerId, input.ElementId);

				if (!IsElementStarted(protocol, eleKey))
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Ignoring RemoveConnections: Unloaded Element:{1} ", protocol.QActionID, eleKey), LogType.Error, LogLevel.NoLogging);
					return false;
				}

				HashSet<int> managedNewByThisProtocol;
				if (!newConnections.TryGetValue(eleKey, out managedNewByThisProtocol))
				{
					managedNewByThisProtocol = new HashSet<int>();
				}

				HashSet<int> managedCurrentByThisProtocol;
				if (!currentConnections.TryGetValue(eleKey, out managedCurrentByThisProtocol))
				{
					managedCurrentByThisProtocol = new HashSet<int>();
				}

				for (int u = 0; u < connectionIDs.Length; u++)
				{
					var con = input.GetConnectionById(connectionIDs[u]);
					if (force || managedCurrentByThisProtocol.Contains(connectionIDs[u]) || managedCurrentByThisProtocol.Contains(-1 * connectionIDs[u]) || managedNewByThisProtocol.Contains(connectionIDs[u]) || managedNewByThisProtocol.Contains(-1 * connectionIDs[u]))
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Connection (" + con.ConnectionId + ")|Deleting Connection:" + con.ConnectionName, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

						if (input.DeleteConnection(connectionIDs[u], bothConnections))
						{
							managedNewByThisProtocol.Remove(connectionIDs[u]);
							managedCurrentByThisProtocol.Remove(connectionIDs[u]);
							managedNewByThisProtocol.Remove(-1 * connectionIDs[u]);
							managedCurrentByThisProtocol.Remove(-1 * connectionIDs[u]);
						}
						else
						{
							protocol.Log(string.Format("QA{0}:|ERR: DCF Connection (" + connectionIDs[u] + ")| Removing DCF Connection:{1} Returned False. Connection may not have been Removed", protocol.QActionID, connectionIDs[u]), LogType.Error, LogLevel.NoLogging);
							finalResult = false;
						}
					}
				}

				newConnections[eleKey] = managedNewByThisProtocol;
				currentConnections[eleKey] = managedCurrentByThisProtocol;

				return finalResult;
			}
			catch (Exception e)
			{
				protocol.Log(string.Format("QA{0}:|ERR: DCF Connection| (Exception) Value at {1} with Exception:{2}", protocol.QActionID, "RemoveConnections", e.ToString()), LogType.Error, LogLevel.NoLogging);
			}

			return false;
		}

		/// <summary>
		/// Remove all Connections with the provided ID.
		/// <para/>Returns: False, if the Remove Failed. (Note, this will be logged as an ERR in protocol logging).
		/// </summary>
		/// <param name="dataMinerID">DataMiner ID containing the connections.</param>
		/// <param name="elementID">Element ID Containing the connections</param>
		/// <param name="bothConnections">For external connections, indicate if the connections on both elements must be deleted.</param>
		/// <param name="force">Indicate if you want to force delete the connection, without using the Mapping Parameters</param>
		/// <param name="connectionIDs">One or more connection IDs to remove</param>
		/// <returns>Boolean indicating the success of the removal</returns>
		//[DISCodeLibrary(Version = 1)]
		public bool RemoveConnections(int dataMinerID, int elementID, bool bothConnections, bool force, params int[] connectionIDs)
		{
			try
			{
				if (currentConnectionsPID == -1)
				{
					protocol.Log("QA" + protocol.QActionID + "|ERR: DCF Connection|DCFHelper Error: Using RemoveConnections requires the CurrentConnectionsPID to be defined! Please change the Options Objects to include this PID", LogType.Error, LogLevel.NoLogging);
					return false;
				}

				bool finalResult = true;
				string eleKey = CreateElementKey(dataMinerID, elementID);
				if (unloadedElements.Contains(eleKey))
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Connection|Ignoring RemoveConnections: Unloaded Element:{1} ", protocol.QActionID, eleKey), LogType.Error, LogLevel.NoLogging);
					return false;
				}

				HashSet<int> managedNewByThisProtocol;
				if (!newConnections.TryGetValue(eleKey, out managedNewByThisProtocol))
				{
					managedNewByThisProtocol = new HashSet<int>();
				}

				HashSet<int> managedCurrentByThisProtocol;
				if (!currentConnections.TryGetValue(eleKey, out managedCurrentByThisProtocol))
				{
					managedCurrentByThisProtocol = new HashSet<int>();
				}

				for (int u = 0; u < connectionIDs.Length; u++)
				{
					if (force || managedCurrentByThisProtocol.Contains(connectionIDs[u]) || managedCurrentByThisProtocol.Contains(-1 * connectionIDs[u]) || managedNewByThisProtocol.Contains(connectionIDs[u]) || managedNewByThisProtocol.Contains(-1 * connectionIDs[u]))
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Connection (" + connectionIDs[u] + ")|Deleting Connection:" + connectionIDs[u], LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

						if (protocol.DeleteConnectivityConnection(connectionIDs[u], dataMinerID, elementID, bothConnections))
						{
							managedNewByThisProtocol.Remove(connectionIDs[u]);
							managedCurrentByThisProtocol.Remove(connectionIDs[u]);
						}
						else
						{
							protocol.Log(string.Format("QA{0}: |ERR: DCF Connection (" + connectionIDs[u] + ")| Removing DCF Connection:{1} Returned False. Connection may not have been Removed", protocol.QActionID, connectionIDs[u]), LogType.Error, LogLevel.NoLogging);
							finalResult = false;
						}
					}
				}

				newConnections[eleKey] = managedNewByThisProtocol;
				currentConnections[eleKey] = managedCurrentByThisProtocol;

				return finalResult;
			}
			catch (Exception e)
			{
				protocol.Log(string.Format("QA{0}:|ERR: DCF Connection| (Exception) Value at {1} with Exception:{2}", protocol.QActionID, "RemoveConnections", e.ToString()), LogType.Error, LogLevel.NoLogging);
			}

			return false;
		}

		/// <summary>
		/// Saves a collection of ConnectivityConnectionProperty objects to a given ConnectivityConnection.
		/// <para/>Returns: An array of SaveConnectionPropertyResults in the same order as the requests. If one of the Saves failed, the returned object will indicate Success: false.
		/// </summary>
		/// <param name="forceRefresh">Force a Refresh of the properties for each connection.</param>
		/// <param name="connection">ConnectivityConnection to save the properties to</param>
		/// <param name="requests">A list of ConnectionPropertyRequests</param>
		/// <returns>An array of SaveConnectionPropertyResults in the same order as the requests. If one of the Saves failed, the returned object will indicate Success: false.</returns>
		//[DISCodeLibrary(Version = 1)]
		public DcfSaveConnectionPropertyResult[] SaveConnectionProperties(bool forceRefresh, ConnectivityConnection connection, params DcfSaveConnectionPropertyRequest[] requests)
		{
			DcfSaveConnectionPropertyResult[] result = new DcfSaveConnectionPropertyResult[requests.Length];
			for (int i = 0; i < requests.Length; i++)
			{
				result[i] = new DcfSaveConnectionPropertyResult(false);
			}

			if (currentConnectionPropertyPID == -1)
			{
				protocol.Log("QA" + protocol.QActionID + "|ERR: DCF Connection Property|DCFHelper Error: Using SaveConnectionProperties requires the CurrentConnectionsPropertiesPID to be defined! Please change the Options Objects to include this PID", LogType.Error, LogLevel.NoLogging);
				return result;
			}

			if (connection == null)
			{
				protocol.Log(string.Format("QA{0}: |ERR: DCF Connection Property|ConnectionPropertyRequest Had empty Connection. The Requested Property Save was not performed.", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
				return result;
			}

			bool externalConnection = connection.SourceDataMinerId != connection.DestinationDMAId || connection.SourceElementId != connection.DestinationEId;
			string connectionIdentifier = connection.ConnectionId + "-" + connection.SourceDataMinerId + "/" + connection.SourceElementId;
			Comparers.CustomComparer<ConnectivityConnectionProperty> propertyIdentifier = new Comparers.CustomComparer<ConnectivityConnectionProperty>(p => p.ConnectionPropertyId);
			string element = connection.SourceDataMinerId + "/" + connection.SourceElementId;

			if (!IsElementStarted(protocol, element))
			{
				protocol.Log(string.Format("QA{0}: |ERR: DCF Connection Property|Ignoring SaveConnectionProperties: Unloaded Element:{1} ", protocol.QActionID, element), LogType.Error, LogLevel.NoLogging);
				return result;
			}

			Collections.FastCollection<ConnectivityConnectionProperty> connectionProperties;
			// Retrieve all properties for this connection in a single call, if they haven't already been called earlier.
			// Check if anything for the element has ever been retrieved
			if (!cachedConnectionPropertiesPerElement.TryGetValue(element, out connectionProperties))
			{
				var itfProps = connection.ConnectionProperties;
				connectionProperties = new Collections.FastCollection<ConnectivityConnectionProperty>(itfProps.Values.ToList());
				itfProps = null;
				cachedConnectionPropertiesPerElement.Add(element, connectionProperties);
			}
			else if (!polledConnectionProperties.Contains(connectionIdentifier))
			{
				// Check if this specific connection has been 'polled'
				var itfProps = connection.ConnectionProperties;
				connectionProperties.Add(itfProps.Values.ToList(), propertyIdentifier);
			}

			List<ConnectivityConnectionProperty> allNewAdded = new List<ConnectivityConnectionProperty>();
			for (int i = 0; i < requests.Length; i++)
			{
				DcfSaveConnectionPropertyRequest currentRequest = requests[i];
				if (currentRequest == null)
				{
					continue;
				}

				if (currentRequest.Name == null || currentRequest.Type == null || currentRequest.Value == null)
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Connection Property|ConnectionPropertyRequest Had empty Name, Type or Value. The Requested Property Save was not performed.", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
					continue;
				}

				bool fixedProperty = currentRequest.FixedProperty;
				bool async = currentRequest.Async;

				// Check if the Property already exists.
				// Based on the Name of the property for this specific connection
				string uniqueKey;
				Expression<Func<ConnectivityConnectionProperty, object>> indexer;
				indexer = p => p.Connection.ConnectionId + "/" + p.ConnectionPropertyName;
				uniqueKey = connection.ConnectionId + "/" + currentRequest.Name;

				connectionProperties.AddIndex(indexer);
				var prop = connectionProperties.FindValue(indexer, uniqueKey).FirstOrDefault();

				ConnectivityConnectionProperty newConnectProp = new ConnectivityConnectionProperty { Connection = connection, ConnectionPropertyName = currentRequest.Name, ConnectionPropertyType = currentRequest.Type, ConnectionPropertyValue = currentRequest.Value };

				if (prop == null)
				{
					// Add New Property
					// ADD PROPERTY
					// Note if external, software will auto-sync the properties by default

					DebugLog("QA" + protocol.QActionID + "|DCF Connection Property|Adding Connection Property:" + newConnectProp.ConnectionPropertyName + ":" + newConnectProp.ConnectionPropertyValue, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

					if (!async)
					{
						ConnectivityConnectionProperty propResult;
						if (connection.AddProperty(newConnectProp.ConnectionPropertyName, newConnectProp.ConnectionPropertyType, newConnectProp.ConnectionPropertyValue, out propResult, 420000))
						{
							newConnectProp.ConnectionPropertyId = propResult.ConnectionPropertyId;
							result[i] = new DcfSaveConnectionPropertyResult(true, newConnectProp);
							allNewAdded.Add(newConnectProp);

							DebugLog("QA" + protocol.QActionID + "|DCF Connection Property (" + newConnectProp.ConnectionPropertyId + ")|Property Added Id:" + newConnectProp.ConnectionPropertyId, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);
						}
						else
						{
							protocol.Log(string.Format("QA{0}:|ERR: DCF Connection Property| Adding Connection Property:{1} Timed out after 7 Minutes or returned false! Property may not have been Added!", protocol.QActionID, newConnectProp.ConnectionPropertyName + ":" + newConnectProp.ConnectionPropertyValue), LogType.Error, LogLevel.NoLogging);
						}
					}
					else
					{
						int outID;
						if (connection.AddProperty(newConnectProp.ConnectionPropertyName, newConnectProp.ConnectionPropertyType, newConnectProp.ConnectionPropertyValue, out outID))
						{
							newConnectProp.ConnectionPropertyId = outID;
							result[i] = new DcfSaveConnectionPropertyResult(true, newConnectProp);
							allNewAdded.Add(newConnectProp);

							DebugLog("QA" + protocol.QActionID + "|DCF Connection Property (" + newConnectProp.ConnectionPropertyId + ")|Property Getting Added (Async) Id:" + newConnectProp.ConnectionPropertyId, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);
						}
						else
						{
							protocol.Log(string.Format("QA{0}:|ERR: DCF Connection Property| Adding Connection Property -Async- :{1} Returned False! Property may not have been Added!", protocol.QActionID, newConnectProp.ConnectionPropertyName + ":" + newConnectProp.ConnectionPropertyValue), LogType.Error, LogLevel.NoLogging);
						}
					}
				}
				else
				{
					// Update existing Property
					// Check if Update is Necessary
					// UPDATE PROPERTY
					if (prop.ConnectionPropertyName == newConnectProp.ConnectionPropertyName && prop.ConnectionPropertyType == newConnectProp.ConnectionPropertyType && prop.ConnectionPropertyValue == newConnectProp.ConnectionPropertyValue)
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Connection Property (" + prop.ConnectionPropertyId + ")|Not Updating Connection Property:" + prop.ConnectionPropertyId + "/" + newConnectProp.ConnectionPropertyName + ":" + newConnectProp.ConnectionPropertyValue + "-- No Change Detected", LogType.Allways, LogLevel.NoLogging, DcfLogType.Same);
						newConnectProp.ConnectionPropertyId = prop.ConnectionPropertyId;
						result[i] = new DcfSaveConnectionPropertyResult(true, newConnectProp);
					}
					else
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Connection Property (" + prop.ConnectionPropertyId + ")|Updating Connection Property:" + prop.ConnectionPropertyId + "/" + newConnectProp.ConnectionPropertyName + ":" + newConnectProp.ConnectionPropertyValue, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

						prop.ConnectionPropertyType = newConnectProp.ConnectionPropertyType;
						prop.ConnectionPropertyValue = newConnectProp.ConnectionPropertyValue;
						if (prop.Update())
						{
							newConnectProp.ConnectionPropertyId = prop.ConnectionPropertyId;
							result[i] = new DcfSaveConnectionPropertyResult(true, newConnectProp);
						}
						else
						{
							protocol.Log(string.Format("QA{0}:|ERR: DCF Connection Property (" + prop.ConnectionPropertyId + ")| Updating Connection Property:{1} Returned False. Property may not have been Updated!", protocol.QActionID, prop.ConnectionPropertyId + "/" + newConnectProp.ConnectionPropertyName + ":" + newConnectProp.ConnectionPropertyValue), LogType.Error, LogLevel.NoLogging);
						}
					}
				}

				if (result[i].Success)
				{
					string eleKey = CreateElementKey(connection.SourceDataMinerId, connection.SourceElementId);
					if (fixedProperty)
					{
						AddToPropertyDictionary(newConnectionProperties, eleKey, -1 * newConnectProp.ConnectionPropertyId);
					}
					else
					{
						AddToPropertyDictionary(newConnectionProperties, eleKey, newConnectProp.ConnectionPropertyId);
					}
				}
			}

			// Add the added ones to the cache
			connectionProperties.Add(allNewAdded, propertyIdentifier);

			return result;
		}

		/// <summary>
		/// Saves a collection of ConnectivityConnectionProperty objects to a given ConnectivityConnection.
		/// <para/>Returns: An array of SaveConnectionPropertyResults in the same order as the requests. If one of the Saves failed, the returned object will indicate Success: false.
		/// </summary>
		/// <param name="connection">ConnectivityConnection to save the properties to</param>
		/// <param name="requests">A list of ConnectionPropertyRequests</param>
		/// <returns>An array of SaveConnectionPropertyResults in the same order as the requests. If one of the Saves failed, the returned object will indicate Success: false.</returns>
		//[DISCodeLibrary(Version = 1)]
		public Connections.DcfSaveConnectionPropertyResult[] SaveConnectionProperties(ConnectivityConnection connection, params Connections.DcfSaveConnectionPropertyRequest[] requests)
		{
			return SaveConnectionProperties(false, connection, requests);
		}

		/// <summary>
		/// Saves a collection of InterfaceProperties to a given ConnectivityInterface
		/// <para/>Returns: An array of SaveInterfacePropertyResults in the same order as the requests. If one of the Saves failed, the returned object will indicate Success: false.
		/// </summary>
		/// <param name="forceRefresh">Force a Refresh of the properties for each Interface.</param>
		/// <param name="connectivityInterface">ConnectivityInterface to save the properties to</param>
		/// <param name="requests">A list of DcfSaveInterfacePropertyRequest</param>
		/// <returns>An array of SaveInterfacePropertyResults in the same order as the requests. If one of the Saves failed, the returned object will indicate Success: false.</returns>
		//[DISCodeLibrary(Version = 1)]
		public DcfSaveInterfacePropertyResult[] SaveInterfaceProperties(bool forceRefresh, ConnectivityInterface connectivityInterface, params DcfSaveInterfacePropertyRequest[] requests)
		{
			DcfSaveInterfacePropertyResult[] result = new DcfSaveInterfacePropertyResult[requests.Length];
			for (int i = 0; i < requests.Length; i++)
			{
				result[i] = new DcfSaveInterfacePropertyResult(false);
			}

			if (currentInterfacesPropertyPID == -1)
			{
				protocol.Log("QA" + protocol.QActionID + "|ERR: DCF Interface Property|DcfHelper Error: Using SaveInterfaceProperties requires the CurrentInterfacesPropertyPID to be defined! Please change the Options Objects to include this PID", LogType.Error, LogLevel.NoLogging);
				return result;
			}

			if (connectivityInterface == null)
			{
				protocol.Log(string.Format("QA{0}: |ERR: DCF Interface Property|InterfacePropertyRequest Had empty Interface. The Requested Property Save was not performed.", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
				return result;
			}

			string interfaceIdentifier = connectivityInterface.InterfaceId + "-" + connectivityInterface.DataMinerId + "/" + connectivityInterface.ElementId;
			Comparers.CustomComparer<ConnectivityInterfaceProperty> propertyIdentifier = new Comparers.CustomComparer<ConnectivityInterfaceProperty>(p => p.InterfacePropertyId);
			string element = connectivityInterface.DataMinerId + "/" + connectivityInterface.ElementId;

			if (!IsElementStarted(protocol, element))
			{
				protocol.Log(string.Format("QA{0}: |ERR: DCF Interface Property|Ignoring SaveInterfaceProperties: Unloaded Element:{1} ", protocol.QActionID, element), LogType.Error, LogLevel.NoLogging);
				return result;
			}

			Collections.FastCollection<ConnectivityInterfaceProperty> interfaceProperties;
			// Retrieve all properties for this interface in a single call, if they haven't already been called earlier.
			// Check if anything for the element has ever been retrieved
			if (!cachedInterfacePropertiesPerElement.TryGetValue(element, out interfaceProperties))
			{
				var itfProps = connectivityInterface.InterfaceProperties;
				interfaceProperties = new Collections.FastCollection<ConnectivityInterfaceProperty>(itfProps.Values.ToList());
				itfProps = null;
				cachedInterfacePropertiesPerElement.Add(element, interfaceProperties);
			}
			else if (!polledInterfaceProperties.Contains(interfaceIdentifier))
			{
				// Check if this specific connection has been 'polled'
				var itfProps = connectivityInterface.InterfaceProperties;
				interfaceProperties.Add(itfProps.Values.ToList(), propertyIdentifier);
			}

			List<ConnectivityInterfaceProperty> allNewAdded = new List<ConnectivityInterfaceProperty>();
			for (int i = 0; i < requests.Length; i++)
			{
				DcfSaveInterfacePropertyRequest currentRequest = requests[i];
				if (currentRequest == null)
				{
					continue;
				}

				if (currentRequest.Name == null || currentRequest.Type == null || currentRequest.Value == null)
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Interface Property|InterfacePropertyRequest Had empty Name, Type or Value. The Requested Property Save was not performed.", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
					continue;
				}

				bool fixedProperty = currentRequest.FixedProperty;
				bool async = currentRequest.Async;

				// Check if the Property already exists.
				// Based on the Name of the property for this specific interface
				string uniqueKey;
				Expression<Func<ConnectivityInterfaceProperty, object>> indexer;
				indexer = p => p.Interface.InterfaceId + "/" + p.InterfacePropertyName;
				uniqueKey = connectivityInterface.InterfaceId + "/" + currentRequest.Name;

				interfaceProperties.AddIndex(indexer);
				var prop = interfaceProperties.FindValue(indexer, uniqueKey).FirstOrDefault();

				ConnectivityInterfaceProperty newInterfaceProp = new ConnectivityInterfaceProperty { Interface = connectivityInterface, InterfacePropertyName = currentRequest.Name, InterfacePropertyType = currentRequest.Type, InterfacePropertyValue = currentRequest.Value };

				if (prop == null)
				{
					// Add New Property
					// ADD PROPERTY
					// Note if external, software will auto-sync the properties by default

					DebugLog("QA" + protocol.QActionID + "|DCF Interface Property|Adding Interface Property:" + newInterfaceProp.InterfacePropertyName + ":" + newInterfaceProp.InterfacePropertyValue, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);
					if (!async)
					{
						ConnectivityInterfaceProperty propResult;
						if (connectivityInterface.AddProperty(newInterfaceProp.InterfacePropertyName, newInterfaceProp.InterfacePropertyType, newInterfaceProp.InterfacePropertyValue, out propResult, 420000))
						{
							newInterfaceProp.InterfacePropertyId = propResult.InterfacePropertyId;
							result[i] = new DcfSaveInterfacePropertyResult(true, newInterfaceProp);
							allNewAdded.Add(newInterfaceProp);

							DebugLog("QA" + protocol.QActionID + "|DCF Interface Property (" + newInterfaceProp.InterfacePropertyId + ")|Property Added Id:" + newInterfaceProp.InterfacePropertyId, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);
						}
						else
						{
							protocol.Log(string.Format("QA{0}:|ERR: DCF Interface Property| Adding Interface Property:{1} Timed out after 7 Minutes or returned false! Property may not have been Added!", protocol.QActionID, newInterfaceProp.InterfacePropertyName + ":" + newInterfaceProp.InterfacePropertyValue), LogType.Error, LogLevel.NoLogging);
						}
					}
					else
					{
						int outID;
						if (connectivityInterface.AddProperty(newInterfaceProp.InterfacePropertyName, newInterfaceProp.InterfacePropertyType, newInterfaceProp.InterfacePropertyValue, out outID))
						{
							newInterfaceProp.InterfacePropertyId = outID;
							result[i] = new DcfSaveInterfacePropertyResult(true, newInterfaceProp);
							allNewAdded.Add(newInterfaceProp);

							DebugLog("QA" + protocol.QActionID + "|DCF Interface Property (" + newInterfaceProp.InterfacePropertyId + ")|Property Getting Added (Async) Id:" + newInterfaceProp.InterfacePropertyId, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);
						}
						else
						{
							protocol.Log(string.Format("QA{0}:|ERR: DCF Interface Property| Adding Interface Property -Async- :{1} Returned False! Property may not have been Added!", protocol.QActionID, newInterfaceProp.InterfacePropertyName + ":" + newInterfaceProp.InterfacePropertyValue), LogType.Error, LogLevel.NoLogging);
						}
					}
				}
				else
				{
					// Update existing Property
					// Check if Update is Necessary
					// UPDATE PROPERTY
					if (prop.InterfacePropertyName == newInterfaceProp.InterfacePropertyName && prop.InterfacePropertyType == newInterfaceProp.InterfacePropertyType && prop.InterfacePropertyValue == newInterfaceProp.InterfacePropertyValue)
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Interface Property (" + prop.InterfacePropertyId + ")|Not Updating Connection Property:" + prop.InterfacePropertyId + "/" + newInterfaceProp.InterfacePropertyName + ":" + newInterfaceProp.InterfacePropertyValue + "-- No Change Detected", LogType.Allways, LogLevel.NoLogging, DcfLogType.Same);
						newInterfaceProp.InterfacePropertyId = prop.InterfacePropertyId;
						result[i] = new DcfSaveInterfacePropertyResult(true, newInterfaceProp);
					}
					else
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Interface Property (" + prop.InterfacePropertyId + ")|Updating Interface Property:" + prop.InterfacePropertyId + "/" + newInterfaceProp.InterfacePropertyName + ":" + newInterfaceProp.InterfacePropertyValue, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

						prop.InterfacePropertyType = newInterfaceProp.InterfacePropertyType;
						prop.InterfacePropertyValue = newInterfaceProp.InterfacePropertyValue;
						if (prop.Update())
						{
							newInterfaceProp.InterfacePropertyId = prop.InterfacePropertyId;
							result[i] = new DcfSaveInterfacePropertyResult(true, newInterfaceProp);
						}
						else
						{
							protocol.Log(string.Format("QA{0}:|ERR: DCF Interface Property (" + prop.InterfacePropertyId + ")| Updating Interface Property:{1} Returned False. Property may not have been Updated!", protocol.QActionID, prop.InterfacePropertyId + "/" + newInterfaceProp.InterfacePropertyName + ":" + newInterfaceProp.InterfacePropertyValue), LogType.Error, LogLevel.NoLogging);
						}
					}
				}

				if (result[i].Success)
				{
					string eleKey = CreateElementKey(connectivityInterface.DataMinerId, connectivityInterface.ElementId);
					if (fixedProperty)
					{
						AddToPropertyDictionary(newConnectionProperties, eleKey, -1 * newInterfaceProp.InterfacePropertyId);
					}
					else
					{
						AddToPropertyDictionary(newConnectionProperties, eleKey, newInterfaceProp.InterfacePropertyId);
					}
				}
			}

			// Add the added ones to the cache
			interfaceProperties.Add(allNewAdded, propertyIdentifier);

			return result;
		}

		/// <summary>
		/// Saves a collection of InterfaceProperties to a given ConnectivityInterface
		/// <para/>Returns: An array of SaveInterfacePropertyResults in the same order as the requests. If one of the Saves failed, the returned object will indicate Success: false.
		/// </summary>
		/// <param name="connectivityInterface">ConnectivityInterface to save the properties to</param>
		/// <param name="requests">A list of DcfSaveInterfacePropertyRequest</param>
		/// <returns>An array of SaveInterfacePropertyResults in the same order as the requests. If one of the Saves failed, the returned object will indicate Success: false.</returns>
		//[DISCodeLibrary(Version = 1)]
		public DcfSaveInterfacePropertyResult[] SaveInterfaceProperties(ConnectivityInterface connectivityInterface, params DcfSaveInterfacePropertyRequest[] requests)
		{
			return SaveInterfaceProperties(false, connectivityInterface, requests);
		}

		/// <summary>
		/// Removes all Connection Properties with the given IDs for a specific Connection.
		///<para/> Return: A boolean indicating if all deletes were successful. (a failure will be logged as ERR in logging)
		/// </summary>
		/// <param name="connection">The ConnectivityConnection Object holding the properties</param>
		/// <param name="force">Indicates if it should force delete all given IDs without checking if they are Managed by this element.</param>
		/// <param name="propertyIDs">One or more Property IDs for the Properties to Delete.</param>
		/// <returns>A boolean indicating if all deletes were successful. (a failure will be logged as ERR in logging)</returns>
		//[DISCodeLibrary(Version = 1)]
		public bool RemoveConnectionProperties(ConnectivityConnection connection, bool force, params int[] propertyIDs)
		{
			try
			{
				bool nullInputDetected = false;
				if (connection == null)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Connection Property| Remove Connection Properties ConnectivityConnection connection was Null", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
					nullInputDetected = true;
				}

				if (nullInputDetected)
				{
					return false;
				}

				if (currentConnectionPropertyPID == -1)
				{
					protocol.Log("QA" + protocol.QActionID + "|ERR: DCF Connection Property|DCFHelper Error: Using RemoveConnectionProperties requires the CurrentConnectionPropertiesPID to be defined! Please change the Options Objects to include this PID", LogType.Error, LogLevel.NoLogging);
					return false;
				}

				bool success = true;

				string eleKey = CreateElementKey(connection.SourceDataMinerId, connection.SourceElementId);
				if (!IsElementStarted(protocol, eleKey))
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Connection Property|Ignoring RemoveConnectionProperties: Unloaded Element:{1} ", protocol.QActionID, eleKey), LogType.Error, LogLevel.NoLogging);
					return false;
				}

				HashSet<int> managedNewByThisProtocol;
				if (!newConnectionProperties.TryGetValue(eleKey, out managedNewByThisProtocol))
				{
					managedNewByThisProtocol = new HashSet<int>();
				}

				HashSet<int> managedCurrentByThisProtocol;
				if (!currentConnectionProperties.TryGetValue(eleKey, out managedCurrentByThisProtocol))
				{
					managedCurrentByThisProtocol = new HashSet<int>();
				}

				foreach (int propertyID in propertyIDs)
				{
					if (force || managedNewByThisProtocol.Contains(propertyID) || managedNewByThisProtocol.Contains(-1 * propertyID) || managedCurrentByThisProtocol.Contains(propertyID) || managedCurrentByThisProtocol.Contains(-1 * propertyID))
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Connection Property (" + propertyID + ")|Deleting Connection Property:" + propertyID, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

						if (!connection.DeleteProperty(propertyID))
						{
							success = false;
							protocol.Log(string.Format("QA{0}:|ERR: DCF Connection Property (" + propertyID + ")| Removing Connection Property:{1} Returned False! Property may not have been Removed!", protocol.QActionID, propertyID), LogType.Error, LogLevel.NoLogging);
						}
						else
						{
							managedCurrentByThisProtocol.Remove(propertyID);
							managedNewByThisProtocol.Remove(propertyID);
							managedCurrentByThisProtocol.Remove(-1 * propertyID);
							managedNewByThisProtocol.Remove(-1 * propertyID);
						}
					}
				}

				newConnectionProperties[eleKey] = managedNewByThisProtocol;
				currentConnectionProperties[eleKey] = managedCurrentByThisProtocol;
				return success;
			}
			catch (Exception e)
			{
				protocol.Log(string.Format("QA{0}:|ERR: DCF Connection Property|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "RemoveConnectionProperties", e.ToString()), LogType.Error, LogLevel.NoLogging);
			}

			return false;
		}

		/// <summary>
		/// Removes all Interface Properties with the given IDs for a specific Connection.
		/// <para/>Returns: A boolean indicating if all deletes were successful. (a failure will be logged as ERR in logging).
		/// </summary>
		/// <param name="itf">The ConnectivityInterface Object holding the properties.</param>
		/// <param name="force">Indicates if it should force delete all given IDs without checking if they are Managed by this element.</param>
		/// <param name="propertyIDs">One or more Property IDs for the Properties to Delete.</param>
		/// <returns> A boolean indicating if all deletes were successful. (a failure will be logged as ERR in logging).</returns>
		//[DISCodeLibrary(Version = 1)]
		public bool RemoveInterfaceProperties(ConnectivityInterface itf, bool force, params int[] propertyIDs)
		{
			try
			{
				bool nullInputDetected = false;
				if (itf == null)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Interface Property|Remove Interface Properties ConnectivityInterface itf was Null", protocol.QActionID), LogType.Error, LogLevel.NoLogging);
					nullInputDetected = true;
				}

				if (nullInputDetected)
				{
					return false;
				}

				if (currentInterfacesPropertyPID == -1)
				{
					protocol.Log("QA" + protocol.QActionID + "|ERR: DCF Interface Property|DCFHelper Error: Using RemoveInterfaceProperties requires the CurrentInterfacePropertiesPID to be defined! Please change the Options Objects to include this PID", LogType.Error, LogLevel.NoLogging);
					return false;
				}

				bool success = true;
				string eleKey = CreateElementKey(itf.DataMinerId, itf.ElementId);

				if (!IsElementStarted(protocol, eleKey))
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Interface Property|Ignoring RemoveInterfaceProperties: Unloaded Element:{1} ", protocol.QActionID, eleKey), LogType.Error, LogLevel.NoLogging);
					return false;
				}

				HashSet<int> managedNewByThisProtocol;
				if (!newInterfaceProperties.TryGetValue(eleKey, out managedNewByThisProtocol))
				{
					managedNewByThisProtocol = new HashSet<int>();
				}

				HashSet<int> managedCurrentByThisProtocol;
				if (!currentInterfaceProperties.TryGetValue(eleKey, out managedCurrentByThisProtocol))
				{
					managedCurrentByThisProtocol = new HashSet<int>();
				}

				foreach (int propertyID in propertyIDs)
				{
					if (force || managedNewByThisProtocol.Contains(propertyID) || managedNewByThisProtocol.Contains(-1 * propertyID) || managedCurrentByThisProtocol.Contains(propertyID) || managedCurrentByThisProtocol.Contains(-1 * propertyID))
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Interface Property (" + propertyID + ")|Deleting Interface Property:" + propertyID, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

						if (!itf.DeleteProperty(propertyID))
						{
							success = false;
							protocol.Log(string.Format("QA{0}:|ERR: DCF Interface Property (" + propertyID + ")| Removing Interface Property:{1} Returned False! Property may not have been Removed!", protocol.QActionID, propertyID), LogType.Error, LogLevel.NoLogging);
						}
						else
						{
							managedCurrentByThisProtocol.Remove(propertyID);
							managedNewByThisProtocol.Remove(propertyID);
							managedCurrentByThisProtocol.Remove(-1 * propertyID);
							managedNewByThisProtocol.Remove(-1 * propertyID);
						}
					}
				}

				newInterfaceProperties[eleKey] = managedNewByThisProtocol;
				currentInterfaceProperties[eleKey] = managedCurrentByThisProtocol;
				return success;
			}
			catch (Exception e)
			{
				protocol.Log(string.Format("QA{0}:|ERR: DCF Interface Property|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "RemoveInterfaceProperties", e.ToString()), LogType.Error, LogLevel.NoLogging);
			}

			return false;
		}

		/// <summary>
		/// Remove all ConnectionProperties for a set of IDs
		/// <para/>Returns: Boolean indicating the success of the removal. (a failure will be logged as ERR in logging)
		/// </summary>
		/// <param name="dataMinerID">DataMiner ID containing the Properties.</param>
		/// <param name="elementID">Element ID Containing the Properties</param>
		/// <param name="force">Indicate if you want to force delete the Property, without using the Mapping Parameters</param>
		/// <param name="propertyIDs">One or more Property IDs to remove</param>
		/// <returns>Boolean indicating the success of the removal. (a failure will be logged as ERR in logging).</returns>
		//[DISCodeLibrary(Version = 1)]
		public bool RemoveConnectionProperties(int dataMinerID, int elementID, bool force, params int[] propertyIDs)
		{
			try
			{
				if (currentConnectionPropertyPID == -1)
				{
					protocol.Log("QA" + protocol.QActionID + "|ERR: DCF Connection Property|DCFHelper Error: Using RemoveConnectionProperties requires the CurrentConnectionPropertiesPID to be defined! Please change the Options Objects to include this PID", LogType.Error, LogLevel.NoLogging);
					return false;
				}

				bool success = true;

				string eleKey = CreateElementKey(dataMinerID, elementID);

				if (!IsElementStarted(protocol, eleKey))
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Connection Property|Ignoring RemoveConnectionProperties (2): Unloaded Element:{1} ", protocol.QActionID, eleKey), LogType.Error, LogLevel.NoLogging);
					return false;
				}

				HashSet<int> managedNewByThisProtocol;
				if (!newConnectionProperties.TryGetValue(eleKey, out managedNewByThisProtocol))
				{
					managedNewByThisProtocol = new HashSet<int>();
				}

				HashSet<int> managedCurrentByThisProtocol;
				if (!currentConnectionProperties.TryGetValue(eleKey, out managedCurrentByThisProtocol))
				{
					managedCurrentByThisProtocol = new HashSet<int>();
				}

				foreach (int propertyID in propertyIDs)
				{
					if (force || managedNewByThisProtocol.Contains(propertyID) || managedNewByThisProtocol.Contains(-1 * propertyID) || managedCurrentByThisProtocol.Contains(propertyID) || managedCurrentByThisProtocol.Contains(-1 * propertyID))
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Connection Property (" + propertyID + ")|Deleting Connection Property:" + propertyID, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

						if (!protocol.DeleteConnectivityConnectionProperty(propertyID, dataMinerID, elementID))
						{
							success = false;
							protocol.Log(string.Format("QA{0}:|ERR: DCF Connection Property (" + propertyID + ")| Removing Connection Property:{1} Returned False! Property may not have been Removed!", protocol.QActionID, propertyID), LogType.Error, LogLevel.NoLogging);
						}
						else
						{
							managedCurrentByThisProtocol.Remove(propertyID);
							managedNewByThisProtocol.Remove(propertyID);
						}
					}
				}

				newConnectionProperties[eleKey] = managedNewByThisProtocol;
				currentConnectionProperties[eleKey] = managedCurrentByThisProtocol;
				return success;
			}
			catch (Exception e)
			{
				protocol.Log(string.Format("QA{0}:|ERR: DCF Connection Property|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "RemoveConnectionProperties", e.ToString()), LogType.Error, LogLevel.NoLogging);
			}

			return false;
		}

		/// <summary>
		/// Remove all InterfaceProperties for a set of IDs
		/// <para/>Returns: Boolean indicating the success of the removal. (a failure will be logged as ERR in logging).
		/// </summary>
		/// <param name="dataMinerID">DataMiner ID containing the Properties.</param>
		/// <param name="elementID">Element ID Containing the Properties</param>
		/// <param name="force">Indicate if you want to force delete the Property, without using the Mapping Parameters</param>
		/// <param name="propertyIDs">One or more Property IDs to remove</param>
		/// <returns>>Boolean indicating the success of the removal. (a failure will be logged as ERR in logging).</returns>
		//[DISCodeLibrary(Version = 1)]
		public bool RemoveInterfaceProperties(int dataMinerID, int elementID, bool force, params int[] propertyIDs)
		{
			bool success = false;
			try
			{
				if (currentInterfacesPropertyPID == -1)
				{
					protocol.Log("QA" + protocol.QActionID + "|ERR: DCF Interface Property|DCFHelper Error: Using RemoveInterfaceProperties requires the CurrentInterfacePropertiesPID to be defined! Please change the Options Objects to include this PID", LogType.Error, LogLevel.NoLogging);
					return false;
				}

				success = true;
				string eleKey = CreateElementKey(dataMinerID, elementID);
				if (!IsElementStarted(protocol, eleKey))
				{
					protocol.Log(string.Format("QA{0}: |ERR: DCF Interface Property|Ignoring RemoveInterfaceProperties (2): Unloaded Element:{1} ", protocol.QActionID, eleKey), LogType.Error, LogLevel.NoLogging);
					return false;
				}

				HashSet<int> managedNewByThisProtocol;
				if (!newInterfaceProperties.TryGetValue(eleKey, out managedNewByThisProtocol))
				{
					managedNewByThisProtocol = new HashSet<int>();
				}

				HashSet<int> managedCurrentByThisProtocol;
				if (!currentInterfaceProperties.TryGetValue(eleKey, out managedCurrentByThisProtocol))
				{
					managedCurrentByThisProtocol = new HashSet<int>();
				}

				foreach (int propertyID in propertyIDs)
				{
					if (force || managedNewByThisProtocol.Contains(propertyID) || managedNewByThisProtocol.Contains(-1 * propertyID) || managedCurrentByThisProtocol.Contains(propertyID) || managedCurrentByThisProtocol.Contains(-1 * propertyID))
					{
						DebugLog("QA" + protocol.QActionID + "|DCF Interface Property (" + propertyID + ")|Deleting Interface Property:" + propertyID, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

						if (!protocol.DeleteConnectivityConnectionProperty(propertyID, dataMinerID, elementID))
						{
							success = false;
							protocol.Log(string.Format("QA{0}:|ERR: DCF Interface Property (" + propertyID + ")| Removing Interface Property:{1} Returned False! Property may not have been Removed!", protocol.QActionID, propertyID), LogType.Error, LogLevel.NoLogging);
						}
						else
						{
							managedCurrentByThisProtocol.Remove(propertyID);
							managedNewByThisProtocol.Remove(propertyID);
						}
					}
				}

				newInterfaceProperties[eleKey] = managedNewByThisProtocol;
				currentInterfaceProperties[eleKey] = managedCurrentByThisProtocol;
			}
			catch (Exception e)
			{
				protocol.Log(string.Format("QA{0}:|ERR: DCF Interface Property|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "RemoveInterfaceProperties", e.ToString()), LogType.Error, LogLevel.NoLogging);
			}

			return success;
		}

		/// <summary>
		/// Used to check if an unfiltered parametergroup has their Interfaces created and ready on the local DataMiner.
		/// <para/>Returns: False, if not all interfaces are ready and the query timed out.
		/// </summary>
		/// <param name="dcfTablePid">PID of the Table linked to with dynamicId in the ParameterGroup.</param>
		/// <param name="dcfTableIndexIdx">IDX of the TableKey Column.</param>
		/// <param name="parameterGroupID">The ParameterGroupID.</param>
		/// <param name="secondsToWait">Maximum time in seconds to wait until interfaces are ready.</param>
		/// <param name="threadSleepMilliSeconds">Sleep Time in milliseconds between each check.</param>
		/// <returns>False, if not all interfaces are ready and the query timed out.</returns>
		//[DISCodeLibrary(Version = 1)]
		public bool CheckInterfacesReady(int dcfTablePid, uint dcfTableIndexIdx, int parameterGroupID, int secondsToWait = 120, int threadSleepMilliSeconds = 1000)
		{
			Dictionary<int, ConnectivityInterface> interfacesDictionary = null;
			bool interfacesPresent = false;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			object[] oKeys = (object[])((object[])protocol.NotifyProtocol(321, dcfTablePid, new uint[] { dcfTableIndexIdx }))[0];
			string thisElementKey = protocol.DataMinerID + "/" + protocol.ElementID;

			if (!IsElementStarted(protocol, thisElementKey))
			{
				protocol.Log(string.Format("QA{0}: |ERR: DCF Interface|Ignoring CheckInterfacesReady: Unloaded Element:{1} ", protocol.QActionID, thisElementKey), LogType.Error, LogLevel.NoLogging);
				return false;
			}

			try
			{
				long miliSecondsToWait = secondsToWait * 1000;
				while (sw.ElapsedMilliseconds < miliSecondsToWait && !interfacesPresent)
				{
					try
					{
						interfacesDictionary = protocol.GetConnectivityInterfaces(localDMAID, localEleID);
						HashSet<string> allInterfaceKeys = new HashSet<string>(interfacesDictionary.Where(p => p.Value.ElementKey == thisElementKey && p.Value.DynamicLink == parameterGroupID).Select(p => p.Value.DynamicPK));
						bool currentInterfacesPresent = true;
						if (oKeys.Length != allInterfaceKeys.Count)
						{
							currentInterfacesPresent = false;
						}

						foreach (var oKey in oKeys)
						{
							string sKey = Convert.ToString(oKey);
							if (!allInterfaceKeys.Contains(sKey))
							{
								currentInterfacesPresent = false;
								break;
							}
						}

						if (!currentInterfacesPresent)
						{
							Thread.Sleep(threadSleepMilliSeconds);
						}
						else
						{
							interfacesPresent = true;
						}
					}
					catch (Exception e)
					{
						protocol.Log(string.Format("QA{0}:|ERR: DCF Interface|(Exception) at {1} CheckInterfacesReady: While Loop with Exception:{2}", protocol.QActionID, sw.ElapsedMilliseconds, e.ToString()), LogType.Error, LogLevel.NoLogging);
					}
				}

				if (interfacesPresent)
				{
					var allInterfaces = new Collections.FastCollection<ConnectivityInterface>(interfacesDictionary.Values.ToArray());
					cachedInterfacesPerElement[localElementKey] = allInterfaces;
				}
			}
			catch (Exception e)
			{
				protocol.Log(string.Format("QA{0}:|ERR: DCF Interface|(Exception) at CheckInterfacesReady with Exception:{1}", protocol.QActionID, e.ToString()), LogType.Error, LogLevel.NoLogging);
			}

			return interfacesPresent;
		}

		/// <summary>
		/// Deletes all DCF Connections and Properties that were previously created by this code. It uses the MappingPIDs to figure this out.
		/// <para/>Returns: A boolean indicating the success of the method.
		/// </summary>
		/// <returns>A boolean indicating the success of the method.</returns>
		//[DISCodeLibrary(Version = 1)]
		public bool DeleteAllManagedDCF()
		{
			bool succes = true;

			try
			{
				try
				{
					foreach (var v in currentConnectionProperties)
					{
						if (!IsElementStarted(protocol, v.Key))
						{
							protocol.Log(string.Format("QA{0}: |ERR: DCF DeleteAllManagedDCF|Ignoring Connection Property Cleanup: Unloaded Element:{1} ", protocol.QActionID, v.Key), LogType.Error, LogLevel.NoLogging);
							continue;
						}

						int thisDMAID;
						int thisEleID;
						SplitEleKey(v.Key, out thisDMAID, out thisEleID);
						foreach (int key in v.Value)
						{
							DebugLog("QA" + protocol.QActionID + "|DCF Full Delete|Triggered DCF Clear- Deleting Connection Property:" + key, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

							int poskey = Math.Abs(key);
							protocol.DeleteConnectivityConnectionProperty(poskey, thisDMAID, thisEleID);
						}
					}

					currentConnectionProperties.Clear();
					newConnectionProperties.Clear();
				}
				catch (Exception e)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Full Delete|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "ClearManagedDC: CurrentConnectionProperties", e.ToString()), LogType.Error, LogLevel.NoLogging);
				}

				currentConnectionProperties.Clear();

				try
				{
					foreach (var v in currentInterfaceProperties)
					{
						if (!IsElementStarted(protocol, v.Key))
						{
							protocol.Log(string.Format("QA{0}: |ERR: DCF DeleteAllManagedDCF|Ignoring Interface Property Cleanup: Unloaded Element:{1} ", protocol.QActionID, v.Key), LogType.Error, LogLevel.NoLogging);
							continue;
						}

						int thisDMAID;
						int thisEleID;
						SplitEleKey(v.Key, out thisDMAID, out thisEleID);
						foreach (int key in v.Value)
						{
							DebugLog("QA" + protocol.QActionID + "|DCF Full Delete|Triggered DCF Clear- Deleting Interface Property:" + key, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

							int poskey = Math.Abs(key);
							protocol.DeleteConnectivityInterfaceProperty(poskey, thisDMAID, thisEleID);
						}
					}

					currentInterfaceProperties.Clear();
				}
				catch (Exception e)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Full Delete|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "ClearManagedDC: CurrentInterfaceProperties", e.ToString()), LogType.Error, LogLevel.NoLogging);
				}

				try
				{
					foreach (var v in newConnectionProperties)
					{
						if (!IsElementStarted(protocol, v.Key))
						{
							protocol.Log(string.Format("QA{0}: |ERR: DCF DeleteAllManagedDCF|Ignoring (n) Connection Property Cleanup: Unloaded Element:{1} ", protocol.QActionID, v.Key), LogType.Error, LogLevel.NoLogging);
							continue;
						}

						int thisDMAID;
						int thisEleID;
						SplitEleKey(v.Key, out thisDMAID, out thisEleID);
						foreach (int key in v.Value)
						{
							DebugLog("QA" + protocol.QActionID + "|DCF Full Delete|Triggered DCF Clear- Deleting New Connection Property:" + key, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

							int poskey = Math.Abs(key);
							protocol.DeleteConnectivityConnectionProperty(poskey, thisDMAID, thisEleID);
						}
					}

					newConnectionProperties.Clear();
				}
				catch (Exception e)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Full Delete|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "ClearManagedDC: NewConnectionProperties", e.ToString()), LogType.Error, LogLevel.NoLogging);
				}

				try
				{
					foreach (var v in newInterfaceProperties)
					{
						if (!IsElementStarted(protocol, v.Key))
						{
							protocol.Log(string.Format("QA{0}: |ERR: DCF DeleteAllManagedDCF|Ignoring (n) Interface Property Cleanup: Unloaded Element:{1} ", protocol.QActionID, v.Key), LogType.Error, LogLevel.NoLogging);
							continue;
						}

						int thisDMAID;
						int thisEleID;
						SplitEleKey(v.Key, out thisDMAID, out thisEleID);
						foreach (int key in v.Value)
						{
							DebugLog("QA" + protocol.QActionID + "|DCF Full Delete|Triggered DCF Clear- Deleting New Interface Property:" + key, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

							int poskey = Math.Abs(key);
							protocol.DeleteConnectivityInterfaceProperty(poskey, thisDMAID, thisEleID);
						}
					}

					newInterfaceProperties.Clear();
				}
				catch (Exception e)
				{
					protocol.Log(string.Format("QA{0}:ERR: |DCF Full Delete|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "ClearManagedDC: NewInterfaceProperties", e.ToString()), LogType.Error, LogLevel.NoLogging);
				}

				try
				{
					foreach (var v in newConnections)
					{
						if (!IsElementStarted(protocol, v.Key))
						{
							protocol.Log(string.Format("QA{0}: |ERR: DCF DeleteAllManagedDCF|Ignoring (n) Connection Cleanup: Unloaded Element:{1} ", protocol.QActionID, v.Key), LogType.Error, LogLevel.NoLogging);
							continue;
						}

						int thisDMAID;
						int thisEleID;
						SplitEleKey(v.Key, out thisDMAID, out thisEleID);
						foreach (int key in v.Value)
						{
							DebugLog("QA" + protocol.QActionID + "|DCF Full Delete|Triggered DCF Clear- Deleting New Connection:" + key, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

							int poskey = Math.Abs(key);
							protocol.DeleteConnectivityConnection(poskey, thisDMAID, thisEleID, true);
						}
					}

					newConnections.Clear();
				}
				catch (Exception e)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Full Delete|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "ClearManagedDC: NewConnection", e.ToString()), LogType.Error, LogLevel.NoLogging);
				}

				try
				{
					foreach (var v in currentConnections)
					{
						if (!IsElementStarted(protocol, v.Key))
						{
							protocol.Log(string.Format("QA{0}: |ERR: DCF DeleteAllManagedDCF|Ignoring Connection Cleanup: Unloaded Element:{1} ", protocol.QActionID, v.Key), LogType.Error, LogLevel.NoLogging);
							continue;
						}

						int thisDMAID;
						int thisEleID;
						SplitEleKey(v.Key, out thisDMAID, out thisEleID);
						foreach (int key in v.Value)
						{
							DebugLog("QA" + protocol.QActionID + "|DCF Full Delete|Triggered DCF Clear- Deleting Connection:" + key, LogType.Allways, LogLevel.NoLogging, DcfLogType.Change);

							int poskey = Math.Abs(key);
							protocol.DeleteConnectivityConnection(poskey, thisDMAID, thisEleID, true);
						}
					}

					currentConnections.Clear();
				}
				catch (Exception e)
				{
					protocol.Log(string.Format("QA{0}:|ERR: DCF Full Delete|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "ClearManagedDC: CurrentConnections", e.ToString()), LogType.Error, LogLevel.NoLogging);
				}
			}
			catch (Exception e)
			{
				protocol.Log(string.Format("QA{0}:|ERR: DCF Full Delete|(Exception) Value at {1} with Exception:{2}", protocol.QActionID, "ClearManagedDCF", e.ToString()), LogType.Error, LogLevel.NoLogging);
				succes = false;
			}

			return succes;
		}

		/// <summary>
		/// The dispose method
		/// </summary>
		//[DISCodeLibrary(Version = 1)]
		public void Dispose()
		{
			SyncMapping();
		}

		/// <summary>
		/// The InternalExternalChar method
		/// </summary>
		/// <param name="p">The p parameter</param>
		/// <returns>The string type object</returns>
		//[DISCodeLibrary(Version = 1)]
		private string InternalExternalChar(ConnectivityConnection p)
		{
			return p.DestinationDMAId == p.SourceDataMinerId && p.SourceElementId == p.DestinationEId ? "I" : "E";
		}

		/// <summary>
		/// The InternalExternalChar method
		/// </summary>
		/// <param name="p">The p parameter</param>
		/// <param name="type">The type parameter</param>
		/// <returns>The string type object</returns>
		//[DISCodeLibrary(Version = 1)]
		private string InternalExternalChar(ConnectivityConnection p, Connections.ConnectionType type)
		{
			if (type == ConnectionType.Both)
			{
				return "B";
			}

			return p.DestinationDMAId == p.SourceDataMinerId && p.SourceElementId == p.DestinationEId ? "I" : "E";
		}

		/// <summary>
		/// The InternalExternalChar method
		/// </summary>
		/// <param name="p">The p parameter</param>
		/// <returns>The string type object</returns>
		//[DISCodeLibrary(Version = 1)]
		private string InternalExternalChar(DcfSaveConnectionRequest p)
		{
			return p.Source.ElementKey == p.Destination.ElementKey ? "I" : "E";
		}

		/// <summary>
		/// The InternalExternalChar method
		/// </summary>
		/// <param name="src">The src parameter</param>
		/// <param name="dst">The dst parameter</param>
		/// <returns>The string type object</returns>
		//[DISCodeLibrary(Version = 1)]
		private string InternalExternalChar(ConnectivityInterface src, ConnectivityInterface dst)
		{
			if (src.DataMinerId != dst.DataMinerId || src.ElementId != src.ElementId)
			{
				return "E";
			}
			else
			{
				return "I";
			}
		}

		/// <summary>
		/// Syncs all changes done to external Parameters that hold the Property Mappings. This needs to be called to keep track of the properties that are managed by this driver.
		/// </summary>
		//[DISCodeLibrary(Version = 1)]
		private void SyncMapping()
		{
			DebugLog("QA" + protocol.QActionID + "|DCF Starting Sync|", LogType.Allways, LogLevel.NoLogging, DcfLogType.Setup);

			// Add the negative mapping to the newMapping
			if (currentConnectionPropertyPID != -1)
			{
				{
					SyncNegative(currentConnectionProperties, newConnectionProperties);
				}
			}

			if (currentInterfacesPropertyPID != -1)
			{
				SyncNegative(currentInterfaceProperties, newInterfaceProperties);
			}

			if (currentConnectionsPID != -1)
			{
				SyncNegative(currentConnections, newConnections);
			}

			switch (helperType)
			{
				case SyncOption.Custom:
					if (currentConnectionPropertyPID != -1)
					{
						foreach (var v in newConnectionProperties)
						{
							HashSet<int> ids;
							if (currentConnectionProperties.TryGetValue(v.Key, out ids))
							{
								ids.UnionWith(v.Value);
								currentConnectionProperties[v.Key] = ids;
							}
							else
							{
								currentConnectionProperties.Add(v.Key, v.Value);
							}
						}
					}

					if (currentInterfacesPropertyPID != -1)
					{
						foreach (var v in newInterfaceProperties)
						{
							HashSet<int> ids;
							if (currentInterfaceProperties.TryGetValue(v.Key, out ids))
							{
								ids.UnionWith(v.Value);
								currentInterfaceProperties[v.Key] = ids;
							}
							else
							{
								currentInterfaceProperties.Add(v.Key, v.Value);
							}
						}
					}

					if (currentConnectionsPID != -1)
					{
						foreach (var v in newConnections)
						{
							HashSet<int> ids;
							if (currentConnections.TryGetValue(v.Key, out ids))
							{
								ids.UnionWith(v.Value);
								currentConnections[v.Key] = ids;
							}
							else
							{
								currentConnections.Add(v.Key, v.Value);
							}
						}
					}

					SyncToParams();
					break;

				case SyncOption.PollingSync:
					SyncToParams();
					break;

				case SyncOption.EndOfPolling:
					RemoveDeleted();
					if (newInterfacePropertyPID != -1)
					{
						protocol.SetParameter(newInterfacePropertyPID, string.Empty);
					}

					if (newConnectionPropertyPID != -1)
					{
						protocol.SetParameter(newConnectionPropertyPID, string.Empty);
					}

					if (newConnectionsPID != -1)
					{
						protocol.SetParameter(newConnectionsPID, string.Empty);
					}

					if (currentInterfacesPropertyPID != -1)
					{
						PropDictionaryToBuffer(newInterfaceProperties, currentInterfacesPropertyPID);
					}

					if (currentConnectionPropertyPID != -1)
					{
						PropDictionaryToBuffer(newConnectionProperties, currentConnectionPropertyPID);
					}

					if (currentConnectionsPID != -1)
					{
						PropDictionaryToBuffer(newConnections, currentConnectionsPID);
					}

					break;
			}

			//Only save the checked elements that were 'true'
			if (startupCheckPID != -1)
			{
				var checkedTrueElement = checkedElements.Except(unloadedElements);
				string newMap = string.Join(";", checkedTrueElement.ToArray());
				protocol.SetParameter(startupCheckPID, newMap);
			}
		}

		/// <summary>
		/// The SyncNegative method
		/// </summary>
		/// <param name="currentDic">The currentDic parameter</param>
		/// <param name="newDic">The newDic parameter</param>
		//[DISCodeLibrary(Version = 1)]
		private void SyncNegative(Dictionary<string, HashSet<int>> currentDic, Dictionary<string, HashSet<int>> newDic)
		{
			foreach (var v in currentDic)
			{
				HashSet<int> currentHashSet = v.Value;
				HashSet<int> newHashSet;
				if (!newDic.TryGetValue(v.Key, out newHashSet))
				{
					newHashSet = new HashSet<int>();
					newDic.Add(v.Key, newHashSet);
				}

				List<int> removeFromCurrentHashSet = new List<int>(currentHashSet.Count);
				// NewHashSet always has priority, need to check if there are changes between fixed & non-fixed
				// Example in comments with "-1" and "1"
				foreach (var p in currentHashSet)
				{
					if (p < 0)
					{
						if (!newHashSet.Contains(Math.Abs(p)))
						{// Don't add -1 if  a key with "1" was already added to the NEWHashSet
							newHashSet.Add(p);
						}
						else
						{
							// Remove the "-1" from current if the New already contains "1"
							removeFromCurrentHashSet.Add(p);
						}
					}
					else
					{
						// Remove "1" from Current if new already contains a "-1"
						if (newHashSet.Contains(p * -1))
						{
							removeFromCurrentHashSet.Add(p);
						}
					}
				}

				foreach (var remove in removeFromCurrentHashSet)
				{
					currentHashSet.Remove(remove);
				}
			}
		}

		/// <summary>
		/// The RemoveDeleted method
		/// </summary>
		//[DISCodeLibrary(Version = 1)]
		private void RemoveDeleted()
		{
			Dictionary<string, HashSet<int>> interfacesToDelete = new Dictionary<string, HashSet<int>>();
			Dictionary<string, HashSet<int>> connectionPropertiesToDelete = new Dictionary<string, HashSet<int>>();
			Dictionary<string, HashSet<int>> connectionsToDelete = new Dictionary<string, HashSet<int>>();
			Dictionary<string, string> elementStates = new Dictionary<string, string>();
			if (currentConnectionsPID != -1)
			{
				foreach (var currentConnection in currentConnections)
				{
					HashSet<int> internalNewConnections;
					IEnumerable<int> source;
					if (newConnections.TryGetValue(currentConnection.Key, out internalNewConnections))
					{
						// all the current values not in new
						source = currentConnection.Value.Except(internalNewConnections).Where(i => i >= 0);
					}
					else
					{
						source = currentConnection.Value.Where(i => i >= 0);
					}

					HashSet<int> currentToDelete;
					if (connectionsToDelete.TryGetValue(currentConnection.Key, out currentToDelete))
					{
						currentToDelete.UnionWith(source);
					}
					else
					{
						HashSet<int> internalToDelete = new HashSet<int>(source);
						connectionsToDelete.Add(currentConnection.Key, internalToDelete);
					}
				}
			}

			if (currentInterfacesPropertyPID != -1)
			{
				foreach (var currentInterfaceProperty in currentInterfaceProperties)
				{
					HashSet<int> internalNewProps;
					IEnumerable<int> source;
					if (newInterfaceProperties.TryGetValue(currentInterfaceProperty.Key, out internalNewProps))
					{
						// all the current values not in new
						source = currentInterfaceProperty.Value.Except(internalNewProps).Where(i => i >= 0);
					}
					else
					{
						source = currentInterfaceProperty.Value.Where(i => i >= 0);
					}

					HashSet<int> currentToDelete;
					if (interfacesToDelete.TryGetValue(currentInterfaceProperty.Key, out currentToDelete))
					{
						currentToDelete.UnionWith(source);
					}
					else
					{
						HashSet<int> internalToDelete = new HashSet<int>(source);
						interfacesToDelete.Add(currentInterfaceProperty.Key, internalToDelete);
					}
				}
			}

			if (currentConnectionPropertyPID != -1)
			{
				foreach (var currentConnectionProperty in currentConnectionProperties)
				{
					HashSet<int> internalNewProps;
					IEnumerable<int> source;
					if (newConnectionProperties.TryGetValue(currentConnectionProperty.Key, out internalNewProps))
					{
						// all the current values not in new
						source = currentConnectionProperty.Value.Except(internalNewProps).Where(i => i >= 0);
					}
					else
					{
						source = currentConnectionProperty.Value.Where(i => i >= 0);
					}

					HashSet<int> currentToDelete;
					if (connectionPropertiesToDelete.TryGetValue(currentConnectionProperty.Key, out currentToDelete))
					{
						currentToDelete.UnionWith(source);
					}
					else
					{
						HashSet<int> internalToDelete = new HashSet<int>(source);
						connectionPropertiesToDelete.Add(currentConnectionProperty.Key, internalToDelete);
					}
				}
			}

			// Delete Connections (will automatically remove all properties for this connection)
			if (connectionsToDelete.Count > 0)
			{
				foreach (var keyToDelete in connectionsToDelete)
				{
					if (!IsElementStarted(protocol, keyToDelete.Key))
					{
						protocol.Log(string.Format("QA{0}: |ERR: DCF Cleanup|Ignoring Connection Cleanup: Unloaded Element:{1} ", protocol.QActionID, keyToDelete.Key), LogType.Error, LogLevel.NoLogging);
						continue;
					}

					string eleKey = keyToDelete.Key;
					if (keyToDelete.Value.Count > 0)
					{
						int thisDMAID;
						int thisEleID;
						SplitEleKey(eleKey, out thisDMAID, out thisEleID);

						HashSet<int> managedNewByThisProtocol;
						if (!newConnections.TryGetValue(eleKey, out managedNewByThisProtocol))
						{
							managedNewByThisProtocol = new HashSet<int>();
						}

						HashSet<int> managedCurrentByThisProtocol;
						if (!currentConnections.TryGetValue(eleKey, out managedCurrentByThisProtocol))
						{
							managedCurrentByThisProtocol = new HashSet<int>();
						}

						string state;
						if (!elementStates.TryGetValue(eleKey, out state))
						{
							//Replace with Notify 323 (element exists)  and the SLElement 377 check?
							state = GetElementState((uint)thisDMAID, (uint)thisEleID);
						}
						bool deleted = string.IsNullOrEmpty(state);

						bool active = IsElementStarted(protocol, thisDMAID, thisEleID);

						foreach (int key in keyToDelete.Value)
						{
							if (active || deleted)
							{
								if (active)
								{
									protocol.DeleteConnectivityConnection(key, thisDMAID, thisEleID, true);
								}
								DebugLog("QA" + protocol.QActionID + "|DCF Connection (" + key + ")|Sync- Deleted Connection:" + eleKey + "/" + key, LogType.Allways, LogLevel.NoLogging, DcfLogType.Setup);

								managedCurrentByThisProtocol.Remove(key);
								managedNewByThisProtocol.Remove(key);
							}
							else
							{
								managedNewByThisProtocol.Add(key);
							}
						}

						newConnections[eleKey] = managedNewByThisProtocol;

						currentConnections[eleKey] = managedCurrentByThisProtocol;
					}
				}
			}

			if (interfacesToDelete.Count > 0)
			{
				// Delete Interface Properties
				foreach (var keyToDelete in interfacesToDelete)
				{
					if (!IsElementStarted(protocol, keyToDelete.Key))
					{
						protocol.Log(string.Format("QA{0}: |ERR: DCF Cleanup|Ignoring Interface Property Cleanup: Unloaded Element:{1} ", protocol.QActionID, keyToDelete.Key), LogType.Error, LogLevel.NoLogging);
						continue;
					}

					string eleKey = keyToDelete.Key;
					HashSet<int> managedNewByThisProtocol;
					if (keyToDelete.Value.Count > 0)
					{
						if (!newInterfaceProperties.TryGetValue(eleKey, out managedNewByThisProtocol))
						{
							managedNewByThisProtocol = new HashSet<int>();
						}

						HashSet<int> managedCurrentByThisProtocol;
						if (!currentInterfaceProperties.TryGetValue(eleKey, out managedCurrentByThisProtocol))
						{
							managedCurrentByThisProtocol = new HashSet<int>();
						}

						int thisDMAID;
						int thisEleID;
						SplitEleKey(eleKey, out thisDMAID, out thisEleID);

						string state;
						if (!elementStates.TryGetValue(eleKey, out state))
						{
							state = GetElementState((uint)thisDMAID, (uint)thisEleID);
						}

						bool deleted = string.IsNullOrEmpty(state);
						bool active = state == "active";

						foreach (int key in keyToDelete.Value)
						{
							if (active || deleted)
							{
								if (active)
								{
									protocol.DeleteConnectivityInterfaceProperty(key, thisDMAID, thisEleID);
								}

								DebugLog("QA" + protocol.QActionID + "|DCF Interface Property (" + key + ")|Sync- Deleted Interface Property:" + eleKey + "/" + key, LogType.Allways, LogLevel.NoLogging, DcfLogType.Setup);
								managedCurrentByThisProtocol.Remove(key);
								managedNewByThisProtocol.Remove(key);
							}
							else
							{
								managedNewByThisProtocol.Add(key);
							}
						}

						newInterfaceProperties[eleKey] = managedNewByThisProtocol;
						currentInterfaceProperties[eleKey] = managedCurrentByThisProtocol;
					}
				}
			}

			if (connectionPropertiesToDelete.Count > 0)
			{
				foreach (var keyToDelete in connectionPropertiesToDelete)
				{
					if (!IsElementStarted(protocol, keyToDelete.Key))
					{
						protocol.Log(string.Format("QA{0}: |ERR: DCF Cleanup|Ignoring Connection Property Cleanup: Unloaded Element:{1} ", protocol.QActionID, keyToDelete.Key), LogType.Error, LogLevel.NoLogging);
						continue;
					}

					string eleKey = keyToDelete.Key;
					int thisDMAID;
					int thisEleID;
					SplitEleKey(eleKey, out thisDMAID, out thisEleID);

					if (keyToDelete.Value.Count > 0)
					{
						HashSet<int> managedNewByThisProtocol;
						if (!newConnectionProperties.TryGetValue(eleKey, out managedNewByThisProtocol))
						{
							managedNewByThisProtocol = new HashSet<int>();
						}

						HashSet<int> managedCurrentByThisProtocol;
						if (!currentConnectionProperties.TryGetValue(eleKey, out managedCurrentByThisProtocol))
						{
							managedCurrentByThisProtocol = new HashSet<int>();
						}

						string state;
						if (!elementStates.TryGetValue(eleKey, out state))
						{
							state = GetElementState((uint)thisDMAID, (uint)thisEleID);
						}

						bool deleted = string.IsNullOrEmpty(state);
						bool active = state == "active";

						foreach (int key in keyToDelete.Value)
						{
							if (active || deleted)
							{
								if (active)
								{
									protocol.DeleteConnectivityConnectionProperty(key, thisDMAID, thisEleID);
								}

								DebugLog("QA" + protocol.QActionID + "|Connection Property (" + key + ") |Sync- Deleted Connection Property:" + eleKey + "/" + key, LogType.Allways, LogLevel.NoLogging, DcfLogType.Setup);

								managedCurrentByThisProtocol.Remove(key);
								managedNewByThisProtocol.Remove(key);
							}
							else
							{
								managedNewByThisProtocol.Add(key);
							}
						}

						newConnectionProperties[eleKey] = managedNewByThisProtocol;
						currentConnectionProperties[eleKey] = managedCurrentByThisProtocol;
					}
				}
			}
		}

		/// <summary>
		/// The SyncToParams method
		/// </summary>
		//[DISCodeLibrary(Version = 1)]
		private void SyncToParams()
		{
			if (newInterfacePropertyPID != -1)
			{
				PropDictionaryToBuffer(newInterfaceProperties, newInterfacePropertyPID);
			}

			if (newConnectionPropertyPID != -1)
			{
				PropDictionaryToBuffer(newConnectionProperties, newConnectionPropertyPID);
			}

			if (newConnectionsPID != -1)
			{
				PropDictionaryToBuffer(newConnections, newConnectionsPID);
			}

			if (currentInterfacesPropertyPID != -1)
			{
				PropDictionaryToBuffer(currentInterfaceProperties, currentInterfacesPropertyPID);
			}

			if (currentConnectionPropertyPID != -1)
			{
				PropDictionaryToBuffer(currentConnectionProperties, currentConnectionPropertyPID);
			}

			if (currentConnectionsPID != -1)
			{
				PropDictionaryToBuffer(currentConnections, currentConnectionsPID);
			}
		}

		/// <summary>
		/// The PropDictionaryToBuffer method
		/// </summary>
		/// <param name="dic">The dic parameter</param>
		/// <param name="pid">The pid parameter</param>
		//[DISCodeLibrary(Version = 1)]
		private void PropDictionaryToBuffer(Dictionary<string, HashSet<int>> dic, int pid)
		{
			string result = MappingDictionaryToMappingValue(dic);
			DebugLog("QA" + protocol.QActionID + "|DCF New Mapping (" + pid + ")|" + result, LogType.Allways, LogLevel.NoLogging, DcfLogType.Setup);
			protocol.SetParameter(pid, result);
		}

		/// <summary>
		/// Converts a Dictionary containing Key: DmaId/EleID and Value: HashSet of IDs into a ; and / separated string with following format:
		/// DmaId/EleId/id/id/id/id/id...;DmaId/EleId/id/id/id/id/...;...
		/// </summary>
		/// <param name="mappingDictionary">a Dictionary containing Key: DmaId/EleID and Value: HashSet of IDs</param>
		/// <returns>String with format: DmaId/EleId/id/id/id/id/id...;DmaId/EleId/id/id/id/id/...;...</returns>
		public static string MappingDictionaryToMappingValue(Dictionary<string, HashSet<int>> mappingDictionary)
		{
			StringBuilder newBuffer = new StringBuilder();
			if (mappingDictionary != null)
			{
				foreach (var dicEle in mappingDictionary)
				{
					string eleKey = dicEle.Key;
					newBuffer.Append(eleKey);
					foreach (int ipid in dicEle.Value)
					{
						newBuffer.Append('/');
						newBuffer.Append(ipid);
					}

					newBuffer.Append(";");
				}
			}

			return newBuffer.ToString().TrimEnd(';');
		}

		/// <summary>
		/// The IsElementStarted method indicating if an element has fully started or not
		/// </summary>
		/// <param name="protocol">The protocol parameter</param>
		/// <param name="elementKey">The elementKey parameter</param>
		/// <param name="useCache">Indiciates if the cache should be used.</param>
		/// <returns>If the element is started</returns>
		//[DISCodeLibrary(Version = 1)]
		private bool IsElementStarted(SLProtocol protocol, string elementKey, bool useCache = true)
		{
			if (elementKey == "local") elementKey = localElementKey;
			if (!checkedElements.Contains(elementKey) || !useCache)
			{
				if (!elementKey.Contains('/'))
				{
					protocol.Log("QA" + protocol.QActionID + "|ERR: IsElementStarted|ElementKey had incorrect Format: " + elementKey, LogType.Error, LogLevel.NoLogging);
					return false;
				}

				string[] elementKeyA = elementKey.Split('/');
				bool result = IsStartedInAllProcesses(protocol, Convert.ToInt32(elementKeyA[0]), Convert.ToInt32(elementKeyA[1]), elementStartupLoopTimeInSeconds);

				checkedElements.Add(elementKey);
				if (!result)
				{
					unloadedElements.Add(elementKey);
				}
				else
				{
					unloadedElements.Remove(elementKey);
				}

				return result;
			}
			else
			{
				if (unloadedElements.Contains(elementKey))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		/// <summary>
		/// The IsElementStarted method
		/// </summary>
		/// <param name="protocol">The protocol parameter</param>
		/// <param name="dmaID">The dmaID parameter</param>
		/// <param name="eleID">The eleID parameter</param>
		/// <param name="useCache">Indicates if the cache should be used/</param>
		/// <returns>The bool type object</returns>
		//[DISCodeLibrary(Version = 1)]
		private bool IsElementStarted(SLProtocol protocol, int dmaID, int eleID, bool useCache = true)
		{
			string elementKey = dmaID + "/" + eleID;
			return IsElementStarted(protocol, elementKey, useCache);
		}

		//[DISCodeLibrary(Version = 1)]
		private bool IsStartedInAllProcesses(SLProtocol protocol, int dmaID, int eleID, int timeoutSeconds)
		{
			bool result = false;
			try
			{
				uint[] varValue1 = new uint[2];
				varValue1[0] = Convert.ToUInt32(dmaID);
				varValue1[1] = Convert.ToUInt32(eleID);
				result = true;

				DebugLog("QA" + protocol.QActionID + "| DBG Load Check ***** SLElement: " + dmaID + "/" + eleID, LogType.DebugInfo, LogLevel.NoLogging, DcfLogType.Setup);

				result = result && IsElementLoadedInSLElement(protocol, varValue1[0], varValue1[1], timeoutSeconds);
				if (result == false)
				{
					protocol.Log("QA" + protocol.QActionID + "|SLElement Check Failed!", LogType.Error, LogLevel.NoLogging);
					return result;
				}

				DebugLog("QA" + protocol.QActionID + "| DBG Load Check ***** Finished", LogType.DebugInfo, LogLevel.NoLogging, DcfLogType.Setup);
			}
			catch (Exception e)
			{
				result = false;
				protocol.Log(string.Format("QA{0}:|ERR: DCF STARTUP|(Exception) at IsElementStarted:{1} with Exception:{2}", protocol.QActionID, dmaID + "/" + eleID, e.ToString()), LogType.Error, LogLevel.NoLogging);
			}

			return result;
		}

		/// <summary>
		/// The PropertiesBufferToDictionary method
		/// </summary>
		/// <param name="pid">The pid parameter</param>
		/// <param name="propDic">The propDic parameter</param>
		//[DISCodeLibrary(Version = 1)]
		private void PropertiesBufferToDictionary(int pid, Dictionary<string, HashSet<int>> propDic)
		{
			string currentItfsProps = Convert.ToString(protocol.GetParameter(pid));

			DebugLog("QA" + protocol.QActionID + "|DCF Old Mapping (" + pid + ")|" + currentItfsProps, LogType.Allways, LogLevel.NoLogging, DcfLogType.Setup);
			MappingValueToMappingDictionary(currentItfsProps, propDic);
		}

		/// <summary>
		/// Converts a MappingValue with format: DmaId/EleId/id/id/id...;DmaId/EleId/id/id/id/id...;... and adds it to a provided
		/// Dictionary with Key: DmaId/EleId and Value: A HashSet of id's
		/// </summary>
		/// <param name="mappingValue">String with format: DmaId/EleId/id/id/id...;DmaId/EleId/id/id/id/id...;... </param>
		/// <param name="mappingDictionary">Dictionary with Key: DmaId/EleId and Value: A HashSet of id's</param>
		public static void MappingValueToMappingDictionary(string mappingValue, Dictionary<string, HashSet<int>> mappingDictionary)
		{
			foreach (string itfsProp in mappingValue.Split(';'))
			{
				if (itfsProp != string.Empty)
				{
					string eleKey;
					string propKeys;
					SplitElePropKey(itfsProp, out eleKey, out propKeys);
					if (propKeys == string.Empty)
					{
						continue;
					}

					string[] propKeysA = propKeys.Split('/');
					int[] propKeysInt = Array.ConvertAll(propKeysA, p => Convert.ToInt32(p));
					AddToPropertyDictionary(mappingDictionary, eleKey, propKeysInt);
				}
			}
		}

		/// <summary>
		/// The AddToPropertyDictionary method
		/// </summary>
		/// <param name="propertyDictionary">The propertyDictionary parameter</param>
		/// <param name="eleKey">The eleKey parameter</param>
		/// <param name="propIDs">The propIDs parameter</param>
		//[DISCodeLibrary(Version = 1)]
		private static void AddToPropertyDictionary(Dictionary<string, HashSet<int>> propertyDictionary, string eleKey, int[] propIDs)
		{
			HashSet<int> returned;
			if (propertyDictionary.TryGetValue(eleKey, out returned))
			{
				returned.UnionWith(propIDs);
			}
			else
			{
				returned = new HashSet<int>(propIDs);
				propertyDictionary.Add(eleKey, returned);
			}
		}

		/// <summary>
		/// The AddToPropertyDictionary method
		/// </summary>
		/// <param name="propertyDictionary">The propertyDictionary parameter</param>
		/// <param name="eleKey">The eleKey parameter</param>
		/// <param name="propID">The propID parameter</param>
		//[DISCodeLibrary(Version = 1)]
		private static void AddToPropertyDictionary(Dictionary<string, HashSet<int>> propertyDictionary, string eleKey, int propID)
		{
			HashSet<int> returned;
			if (propertyDictionary.TryGetValue(eleKey, out returned))
			{
				returned.Add(propID);
			}
			else
			{
				returned = new HashSet<int>();
				returned.Add(propID);
				propertyDictionary.Add(eleKey, returned);
			}
		}

		/// <summary>
		/// The SplitElePropKey method
		/// </summary>
		/// <param name="itfsProp">The itfsProp parameter</param>
		/// <param name="eleID">The eleID parameter</param>
		/// <param name="propKey">The propKey parameter</param>
		private static void SplitElePropKey(string itfsProp, out string eleID, out string propKey)
		{
			int endOfDmaID = itfsProp.IndexOf('/');
			if (endOfDmaID != -1)
			{
				int endOfEleID = itfsProp.IndexOf('/', endOfDmaID + 1);
				if (endOfEleID == -1)
				{
					eleID = itfsProp;
					propKey = string.Empty;
				}
				else
				{
					eleID = itfsProp.Substring(0, endOfEleID);
					propKey = itfsProp.Substring(endOfEleID + 1);
				}
			}
			else
			{
				eleID = string.Empty;
				propKey = string.Empty;
			}
		}

		/// <summary>
		/// The SplitEleKey method
		/// </summary>
		/// <param name="elementKey">The elementKey parameter</param>
		/// <param name="dmaID">The dmaID parameter</param>
		/// <param name="elementID">The elementID parameter</param>
		/// <returns>The bool type object</returns>
		//[DISCodeLibrary(Version = 1)]
		private static bool SplitEleKey(string elementKey, out int dmaID, out int elementID)
		{
			string[] elementKeyA = elementKey.Split('/');
			if (elementKeyA.Length > 1)
			{
				dmaID = Convert.ToInt32(elementKeyA[0]);
				elementID = Convert.ToInt32(elementKeyA[1]);
				return true;
			}
			else
			{
				dmaID = -1;
				elementID = -1;
				return false;
			}
		}

		/// <summary>
		/// The CreateElementKey method
		/// </summary>
		/// <param name="dataMinerID">The dataMinerID parameter</param>
		/// <param name="eleID">The eleID parameter</param>
		/// <returns>The string type object</returns>
		//[DISCodeLibrary(Version = 1)]
		private static string CreateElementKey(int dataMinerID, int eleID)
		{
			return dataMinerID + "/" + eleID;
		}

		/// <summary>
		/// Allows to know if an element is started and fully loaded in SLElement (and optionally, loop until it is).
		/// </summary>
		/// <param name="protocol"></param>
		/// <param name="iDmaId">ID of the DMA on which the element to be checked is located</param>
		/// <param name="iElementId">ID of the element to be checked</param>
		/// <param name="iSecondsToWait">Number of seconds to wait for the element to be fully loaded.</param>
		/// <returns>True if the element is fully loaded within the given 'iSecondsToWait'</returns>
		//[DISCodeLibrary(Version = 1)]
		private bool IsElementLoadedInSLElement(SLProtocol protocol, uint iDmaId, uint iElementId, int iSecondsToWait = 0)
		{
			bool bIsFullyStarted = false;

			DateTime dtStart = DateTime.Now;
			int iElapsedSeconds = 0;
			string sExceptionThrown = null;

			while (!bIsFullyStarted && iElapsedSeconds <= iSecondsToWait)
			{
				try
				{
					object oResult = protocol.NotifyDataMiner(377/*NT_ELEMENT_STARTUP_COMPLETE*/, new uint[] { iDmaId, iElementId }, null);
					// protocol.Log("QA" + protocol.QActionID + "|IsElementLoadedInSLElement|oResult : " + Convert.ToString(oResult), LogType.Allways, LogLevel.NoLogging);
					if (oResult != null)
					{
						bIsFullyStarted = Convert.ToBoolean(oResult);
					}
					else
					{
						//null means the element is stopped.
						break;
					}
				}
				catch (Exception ex)
				{
					sExceptionThrown = ex.ToString();
				}

				if (!bIsFullyStarted)
				{
					Thread.Sleep(100);
					iElapsedSeconds = (int)(DateTime.Now - dtStart).TotalSeconds;
				}
			}

			if (!bIsFullyStarted && sExceptionThrown != null)
			{
				protocol.Log("QA" + protocol.QActionID + "|ERR: IsElementLoadedInSLElement|Exception thrown :" + Environment.NewLine + sExceptionThrown, LogType.Error, LogLevel.NoLogging);
			}

			return bIsFullyStarted;
		}

		/// <summary>
		/// Get the state of an element (based on SLDMS, so basically this comes pretty much to the same as a IsElementLoadedInSLDMS)
		/// </summary>
		/// <param name="iDmaId">ID of the DMA on which the element from which the state needs to be retrieved is located</param>
		/// <param name="iElementId">ID of the element from which the state needs to be retrieved</param>
		/// <returns>The element state. In case of failure, null is returned.</returns>
		//[DISCodeLibrary(Version = 1)]
		private string GetElementState(uint iDmaId, uint iElementId)
		{
			try
			{
				DMSClass dms = new DMSClass();
				object oState = null;
				dms.Notify(91/*DMS_GET_ELEMENT_STATE*/, 0, iDmaId, iElementId, out oState);
				string sElementState = oState as string;

				return sElementState;
			}
			catch (Exception ex)
			{
				protocol.Log("QA" + protocol.QActionID + "|GetElementState|Exception thrown : " + Environment.NewLine + ex.ToString(), LogType.Error, LogLevel.NoLogging);
				return null;
			}
		}

		//[DISCodeLibrary(Version = 1)]
		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="level"></param>
		/// <param name="levelOfThisLog"></param>
		private void DebugLog(string message, LogType type, LogLevel level, DcfLogType levelOfThisLog)
		{
			switch (debugLevel)
			{
				case DcfDebugLevel.All:
					switch (levelOfThisLog)
					{
						case DcfLogType.Change:
						case DcfLogType.Setup:
						case DcfLogType.Same:
						case DcfLogType.Info:
						default:
							protocol.Log(message, type, level);
							break;
					}

					break;

				case DcfDebugLevel.Changes_And_Setup:
					switch (levelOfThisLog)
					{
						case DcfLogType.Change:
						case DcfLogType.Setup:
						case DcfLogType.Info:
							protocol.Log(message, type, level);
							break;
					}

					break;

				case DcfDebugLevel.Only_Changes:
					switch (levelOfThisLog)
					{
						case DcfLogType.Change:
						case DcfLogType.Info:
							protocol.Log(message, type, level);
							break;
					}

					break;

				case DcfDebugLevel.Only_Setup:
					switch (levelOfThisLog)
					{
						case DcfLogType.Setup:
						case DcfLogType.Info:
							protocol.Log(message, type, level);
							break;
					}

					break;

				case DcfDebugLevel.None:
					break;
			}
		}
	}
}