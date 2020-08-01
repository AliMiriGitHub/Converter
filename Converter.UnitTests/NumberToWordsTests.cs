using NUnit.Framework;
using Services.Utility;

namespace Converter.UnitTests
{
    public class NumberToWordsTests
    {
        NumberToWordsConverter _converter;
        [SetUp]
        public void Setup()
        {
            _converter = new NumberToWordsConverter();
        }

        [Test]
        public void Convert_HavePluralS_AreEqual()
        {
            //Act
            var result = _converter.Convert("0");
            
            //Assert
            Assert.AreEqual(result, "zero dollars");
        }

        [Test]
        public void Convert_DoesNotHavePluralS_AreEqual()
        {
            //Act
            var result = _converter.Convert("1");
            
            //Assert
            Assert.AreEqual(result, "one dollar");
        }

        [Test]
        public void Convert_CentIsTenMultiple_AreEqual()
        {
            //Act
            var result = _converter.Convert("25,1");
            
            //Assert
            Assert.AreEqual(result, "twenty-five dollars and ten cents");
        }

        [Test]
        public void Convert_CentLessThanTen_AreEqual()
        {
            //Act
            var result = _converter.Convert("0,01");

            //Assert
            Assert.AreEqual(result, "zero dollars and one cent");
        }

        [Test]
        public void Convert_NumberWithoutCents_AreEqual()
        {
            //Act
            var result = _converter.Convert("45 100");

            //Assert
            Assert.AreEqual(result, "forty-five thousand one hundred dollars");
        }

        [Test]
        public void Convert_MaxNumber_AreEqual()
        {
            //Act
            var result = _converter.Convert("999 999 999,99");

            //Assert
            Assert.AreEqual(result, "nine hundred ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and ninety-nine cents");
        }

        [Test]
        public void Convert_CentsMoreHundred_AreEqual()
        {
            //Act
            var result = _converter.Convert("1,152");

            //Assert
            Assert.AreEqual(result, "one dollar and fifteen cents");
        }
    }
}