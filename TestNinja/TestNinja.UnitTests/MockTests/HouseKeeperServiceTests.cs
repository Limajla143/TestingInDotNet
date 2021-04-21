﻿using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.MockTests
{
    [TestFixture]
    public class HouseKeeperServiceTests
    {
        private HousekeeperService _service;
        private Mock<IStatementGenerator> _statementGenerator;
        private Mock<IEmailSender> _emailSender;
        private Mock<IXtraMessageBox> _messageBox;
        private DateTime _statementDate = new DateTime(2017, 1, 1);
        private Housekeeper _housekeeper = new Housekeeper();
        private readonly string _statementFileName = "filename";
        [SetUp]
        public void Setup()
        {
            _housekeeper = new Housekeeper
            {
                Email = "a",
                FullName = "b",
                Oid = 1,
                StatementEmailBody = "c"
            };

            var unitOfWork = new Mock<IUnitOfWork>();
           
            unitOfWork.Setup(uow => uow.Query<Housekeeper>()).Returns(new List<Housekeeper>
            {
                _housekeeper
            }.AsQueryable());

            _statementGenerator = new Mock<IStatementGenerator>();
            _emailSender = new Mock<IEmailSender>();
            _messageBox = new Mock<IXtraMessageBox>();

            _service = new HousekeeperService(
                unitOfWork.Object,
                _statementGenerator.Object,
                _emailSender.Object,
                _messageBox.Object);
        }

        [Test]
        public void SendStatementEmails_WhenCalled_GeneralStatements()
        {      
            _service.SendStatementEmails(_statementDate);

            _statementGenerator.Verify(sg => sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName, 
                (_statementDate)));
        }

        [Test]
        public void SendStatementEmails_HouseKeepersEmailIsNull_ShouldNotGenerateStatement()
        {
            _housekeeper.Email = null;

            _service.SendStatementEmails(_statementDate);

            _statementGenerator.Verify(sg => sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName, 
                (_statementDate)), Times.Never);
        }

        [Test]
        public void SendStatementEmails_HouseKeepersEmailIsWhiteSpace_ShouldNotGenerateStatement()
        {
            _housekeeper.Email = " ";

            _service.SendStatementEmails(_statementDate);

            _statementGenerator.Verify(sg => sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName,
                (_statementDate)), Times.Never);
        }

        [Test]
        public void SendStatementEmails_HouseKeepersEmailIsEmpty_ShouldNotGenerateStatement()
        {
            _housekeeper.Email = "";

            _service.SendStatementEmails(_statementDate);

            _statementGenerator.Verify(sg => sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName,
                (_statementDate)), Times.Never);
        }

        [Test]
        public void SendStatementEmails_WhenCalled_ShouldNotGenerateStatement()
        {

            _statementGenerator.Setup(sg => sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName,
                (_statementDate))).Returns(_statementFileName);

            _service.SendStatementEmails(_statementDate);

            _emailSender.Verify(es => es.EmailFile(
                _housekeeper.Email,
                _housekeeper.StatementEmailBody,
                _statementFileName,
                It.IsAny<string>()));
        }

        [Test]
        public void SendStatementEmails_StatementFileNameIsNull_ShouldNotEmailTheStatement()
        {

            _statementGenerator.Setup(sg => sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName,
                (_statementDate))).Returns(() => null);

            _service.SendStatementEmails(_statementDate);

            _emailSender.Verify(es => es.EmailFile(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>()),
               Times.Never);
        }

        [Test]
        public void SendStatementEmails_StatementFileNameEmptyString_ShouldNotEmailTheStatement()
        {

            _statementGenerator.Setup(sg => sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName,
                (_statementDate))).Returns("");

            _service.SendStatementEmails(_statementDate);

            _emailSender.Verify(es => es.EmailFile(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>()),
               Times.Never);
        }

        [Test]
        public void SendStatementEmails_StatementFileNameIsWhitespace_ShouldNotEmailTheStatement()
        {

            _statementGenerator.Setup(sg => sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName,
                (_statementDate))).Returns(" ");

            _service.SendStatementEmails(_statementDate);

            _emailSender.Verify(es => es.EmailFile(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>()),
               Times.Never);
        }
    }
}