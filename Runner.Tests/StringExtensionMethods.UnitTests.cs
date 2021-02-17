using Core.Entities;
using NUnit.Framework;
using Runner;
using Shared;

namespace Runner.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void GetStableHashCode_Works()
        {
            var advertisement = new Advertisement 
            {
                Description = @"12~@#ds./\';",
                ImageUrl = @"!@$#@##^$%^&GHFDHDJGJ:56",
                Price = "123,345",
                Title = "HSDJKH32445hjhj",
                Url = @"https://www.hallo.com?24324=q#23"
            };

            Assert.That(advertisement.GetHashCode(), Is.EqualTo(-1806276902));


            var url = "https://www.marktplaats.nl/l/watersport-en-boten/surfen-golfsurfen/f/fish/8354/#distanceMeters:25000|postcode:2563GP|searchInTitleAndDescription:true";
            Assert.That(url.GetStableHashCode(), Is.EqualTo(39941003));
        }
    }
}