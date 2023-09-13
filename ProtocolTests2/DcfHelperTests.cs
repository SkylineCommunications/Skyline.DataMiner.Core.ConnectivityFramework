using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Skyline.DataMiner.Core.ConnectivityFramework.Protocol;
using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections;
using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Debug;
using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options;
using Skyline.DataMiner.Net;
using Skyline.DataMiner.Net.ResourceManager.Objects;
using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Text;


namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Tests
{
    [TestClass()]
    public class DcfHelperTests
    {
        private DcfHelper dcfHelper;
        private Mock<SLProtocol> protocolMock;
        private Mock<ConnectivityConnection> connectionMock;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            protocolMock = new Mock<SLProtocol>();
            connectionMock = new Mock<ConnectivityConnection>();

            int startupCheckPID = 12345;
            var removalOptions = new MockDcfRemovalOptions();
            DcfDebugLevel debugLevel = DcfDebugLevel.None;

            dcfHelper = new DcfHelper(protocolMock.Object, startupCheckPID, removalOptions, debugLevel);
        }

        [TestMethod]
        public void SaveConnections_NullRequests_ReturnsEmptyResults()
        {
            // Act
            var results = dcfHelper.SaveConnections(false, null);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Length);
        }

        [TestMethod]
        public void SaveConnections_EmptyRequests_ReturnsEmptyResults()
        {
            // Act
            var results = dcfHelper.SaveConnections(false, new DcfSaveConnectionRequest[0]);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Length);
        }

        [TestMethod]
        public void SaveConnections_ConnectionsWithNullSourceOrDestination_LogsErrorAndSkips()
        {
            // Arrange
            var requests = new[]
            {
            new DcfSaveConnectionRequest { Source = null, Destination = new DestinationInfo() },
            new DcfSaveConnectionRequest { Source = new SourceInfo(), Destination = null }
        };

            // Act
            var results = dcfHelper.SaveConnections(false, requests);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Length);

            // Verify that the protocol.Log method was called twice with the expected error message.
            protocolMock.Verify(
                p => p.Log(
                    It.IsAny<string>(),
                    LogType.Error,
                    LogLevel.NoLogging
                ),
                Times.Exactly(2)
            );
        }


    }
}