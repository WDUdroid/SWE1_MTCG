using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SWE1_MTCG;
using Moq;
using SWE1_MTCG.HelperObjects;
using SWE1_MTCG.REST;

namespace SWE1_MTCG_TEST
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void GetHttpContentShortMessageTest()
        {
            var mockTcpHandler = new Mock<ITcpHandler>();
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream) { AutoFlush = true };
            writer.Write("abc");
            memoryStream.Position = 0;


            int callCounter = 0;
            mockTcpHandler
                .Setup(c => c.DataAvailable(It.IsAny<TcpClient>()))
                .Returns(() =>
                {
                    if (callCounter++ < 1)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                });

            mockTcpHandler
                .Setup(c => c.GetStream(It.IsAny<TcpClient>()))
                .Returns(memoryStream);

            WebHandler contentReader = new WebHandler(mockTcpHandler.Object);

            string actualValue = contentReader.GetHttpContent();

            Assert.AreEqual("abc", actualValue);
        }

        [Test]
        public void GetHttpContentLongMessageTest()
        {
            var message = @"GET /messages HTTP/1.1
                            Content - Type: text / plain
                            User - Agent: PostmanRuntime / 7.26.8
                            Accept: */*
                            Postman-Token: 18f78a05-ec79-4a35-8bca-8752b54e96a0
                            Host: localhost:8000
                            Accept-Encoding: gzip, deflate, br
                            Connection: keep-alive
                            Content-Length: 12

                            Long Message";

            var mockTcpHandler = new Mock<ITcpHandler>();
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream) { AutoFlush = true };
            writer.Write(message);
            memoryStream.Position = 0;


            int callCounter = 0;
            mockTcpHandler
                .Setup(c => c.DataAvailable(It.IsAny<TcpClient>()))
                .Returns(() =>
                {
                    if (callCounter++ < 1)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                });

            mockTcpHandler
                .Setup(c => c.GetStream(It.IsAny<TcpClient>()))
                .Returns(memoryStream);

            WebHandler contentReader = new WebHandler(mockTcpHandler.Object);

            string actualValue = contentReader.GetHttpContent();

            Assert.AreEqual(message, actualValue);
        }

        [Test]
        public void GetHttpContentSpecialCharacterMessageTest()
        {
            var mockTcpHandler = new Mock<ITcpHandler>();
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream) { AutoFlush = true };
            writer.Write("ÄÜÖäüö");
            memoryStream.Position = 0;


            int callCounter = 0;
            mockTcpHandler
                .Setup(c => c.DataAvailable(It.IsAny<TcpClient>()))
                .Returns(() =>
                {
                    if (callCounter++ < 1)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                });

            mockTcpHandler
                .Setup(c => c.GetStream(It.IsAny<TcpClient>()))
                .Returns(memoryStream);

            WebHandler contentReader = new WebHandler(mockTcpHandler.Object);

            string actualValue = contentReader.GetHttpContent();

            Assert.AreEqual("????????????", actualValue);
        }

        [Test]
        public void SendHttpContentTest()
        {
            var mockTcpHandler = new Mock<ITcpHandler>();
            var mockRequestContext = new Mock<IRequestContext>();
            var memoryStream = new MemoryStream();
            memoryStream.Position = 0;

            mockTcpHandler
                .Setup(c => c.GetStream(It.IsAny<TcpClient>()))
                .Returns(memoryStream);

            mockRequestContext
                .Setup(c => c.StatusCode)
                .Returns("200 OK");
            mockRequestContext
                .Setup(c => c.ContentType)
                .Returns("text/plain");
            mockRequestContext
                .Setup(c => c.Payload)
                .Returns("SendHttpContentTest");

            WebHandler contentWriter = new WebHandler(mockTcpHandler.Object, mockRequestContext.Object);

            contentWriter.SendHttpContent();

            string expectedValue = "HTTP/1.1 200 OK\r\nServer: MTCG-Server\r\n";
            expectedValue += "Content-Type: text/plain\r\nContent-Length: 19\r\n\r\n";
            expectedValue += "SendHttpContentTest\r\n";

            string actualValue = Encoding.ASCII.GetString(memoryStream.ToArray());

            Console.WriteLine(actualValue);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RequestContextTest()
        {
            var dataList = new List<String>();
            string receivedData = "PUT /messages/3 HTTP/1.1\r\nContent - Type: text / plain\r\n";
            receivedData += "User - Agent: PostmanRuntime / 7.26.8\r\nAccept: */*\r\n";
            receivedData += "Postman-Token: b4dfd06b-7ae4-4529-ab44-b7a202d33ef5\r\n";
            receivedData += "Accept-Encoding: gzip, deflate, br\r\nConnection: keep-alive\r\n";
            receivedData += "Content-Length: 6\r\n\r\nzweite";

            RequestContext processData = new RequestContext(receivedData,dataList, new Battle());

            string actualHttpRequestValue = processData.HeaderInfo["RequestPath"];
            string actualHttpBodyValue = processData.HeaderInfo["Body"];
            string actualHttpVersionValue = processData.HeaderInfo["HttpVersion"];

            Assert.AreEqual("/messages/3", actualHttpRequestValue);
            Assert.AreEqual("zweite", actualHttpBodyValue);
            Assert.AreEqual("HTTP/1.1", actualHttpVersionValue);
        }


        [Test]
        public void RequestContextTestBadRequestEmpty()
        {
            var dataList = new List<String>();
            string receivedData = "";

            RequestContext processData = new RequestContext(receivedData, dataList, new Battle());

            Assert.AreEqual("400 Bad Request", processData.StatusCode);
            Assert.AreEqual("Bad Request", processData.Payload);
            Assert.AreEqual("text/plain", processData.ContentType);
        }

        [Test]
        public void CheckOnBattleRecognizeOneResultTest()
        {
            var battleCenter = new Battle();
            
            battleCenter.Result.Add("TestData");

            Assert.AreEqual("TestData", battleCenter.CheckOnBattle());
            Assert.AreEqual(1, battleCenter.Views);
        }

        [Test]
        public void CheckOnBattleRecognizeTwoResultsTest()
        {
            var battleCenter = new Battle();

            battleCenter.Result.Add("TestData");

            Assert.AreEqual("TestData", battleCenter.CheckOnBattle());
            Assert.AreEqual(1, battleCenter.Views);

            battleCenter.Result.Add("TestData");

            Assert.AreEqual("TestData", battleCenter.CheckOnBattle());
            Assert.AreEqual(0, battleCenter.Views);
        }

        [Test]
        public void CheckOnBattleRecognizeThreeResultsTest()
        {
            var battleCenter = new Battle();

            battleCenter.Result.Add("TestData");
            battleCenter.Result.Add("TestData");
            battleCenter.Result.Add("TestData");

            Assert.AreEqual("TestData", battleCenter.CheckOnBattle());
            Assert.AreEqual(1, battleCenter.Views);
            Assert.AreEqual(3, battleCenter.Result.Count);


            Assert.AreEqual("TestData", battleCenter.CheckOnBattle());
            Assert.AreEqual(0, battleCenter.Views);
            Assert.AreEqual(0, battleCenter.Result.Count);
        }

        [Test]
        public void NewUserTest()
        {
            var battleCenter = new Battle();

            battleCenter.Challengers.Add(new User(new List<CardInBattle>(), "player1"));
            battleCenter.Challengers.Add(new User(new List<CardInBattle>(), "player2"));

            Assert.AreEqual("TestData", battleCenter.CheckOnBattle());
            Assert.AreEqual(1, battleCenter.Views);
            Assert.AreEqual(3, battleCenter.Result.Count);


            Assert.AreEqual("TestData", battleCenter.CheckOnBattle());
            Assert.AreEqual(0, battleCenter.Views);
            Assert.AreEqual(0, battleCenter.Result.Count);
        }

    }
}