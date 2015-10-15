using NUnit.Framework;
using System.Collections;

namespace Landis.Test.Succession
{
    [TestFixture]
    public class BitArray_Test
    {
        [Test]
        public void Or()
        {
            BitArray array1 = new BitArray(4);
            BitArray array2 = new BitArray(4);

            array1[0] = true;
            array1[1] = false;
            array1[2] = false;
            array1[3] = false;

            array2[0] = true;
            array2[1] = false;
            array2[2] = true;
            array2[3] = true;

            array1.Or(array2);

            Assert.AreEqual(true,  array1[0]);
            Assert.AreEqual(false, array1[1]);
            Assert.AreEqual(true,  array1[2]);
            Assert.AreEqual(true,  array1[3]);
        }
    }
}
