# Protocol

## About
This package contains helper code allowing to easily manage DCF (DataMiner Connectivity Framework) connections from a protocol. For more information about DCF, see [DataMiner Connectivity Framework](https://docs.dataminer.services/develop/devguide/Connector/AdvancedDcf.html).
The generated NuGet package can be found on [nuget.org](https://www.nuget.org/packages/Skyline.DataMiner.Core.ConnectivityFramework.Protocol).

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exist. In addition, you can leverage DataMiner Development Packages to build you own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.

<!-- Uncomment below and add more info to provide more information about how to use this package. -->
## Migrating your code from legacy DCFHelper in Precompiled QAction

	- Replace All: DCFHelper  with DcfHelper
	- Replace DCFMappingOptions
		- If EndOfPolling was used: DcfRemovalOptionsAuto
		- If Custom was used: DcfRemovalOptionsManual
		- If BufferSync was used: DcfRemovalOptionsBuffer
	- Remove the line with SyncOption.
	- Replace DCFSaveConnectionResult with DcfSaveConnectionResult
	- Replace DCFSaveConnectionRequest with DcfSaveConnectionRequest
	- Replace DCFDynamicLink  with  DcfInterfaceFilterSingle  or DcfInterfaceFilterMulti
		- Single, when used in SaveConnections
		- Multi or single when used in GetInterfaces (will need a manual check to see what you need to change here)
	- Saving Connection Properties has changed a lot:
		- It can now be added to the DcfSaveConnectionRequest object
		- It uses DcfSaveConnectionPropertyRequest objects
	- If DVEColumn or External is used to do startup checks, this can be removed
	All Startup checks are handled 'Lazy' as the code runs, it will make sure to check for startup when it's needed

