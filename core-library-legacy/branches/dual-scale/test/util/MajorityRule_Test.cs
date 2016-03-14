// Copyright 2007 University of Wisconsin
// Author: James Domingo, UW-Madison, FLEL

using Landis.DualScale;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Collections.Generic;

namespace Landis.Test.Util
{
	[TestFixture]
	public class MajorityRule_Test
	{
		private IDictionary<ushort, int> codeCounts;
        private MajorityRule.Delegates.RandomBetween originalRandomBetween;
        private bool randomBetweenCalled;
        private int expectedLow;
        private int expectedHigh;
        private int randomResult;

		//---------------------------------------------------------------------

		public int RandomBetween(int low,
                                 int high)
		{
            randomBetweenCalled = true;
            Assert.AreEqual(expectedLow, low);
            Assert.AreEqual(expectedHigh, high);
            return randomResult;
        }

		//---------------------------------------------------------------------

        public int RandomBetweenFail(int low,
                                     int high)
        {
            Assert.Fail("MajorityRule.SelectMapCode called random number generator");
            return 0;  // Not reached
        }

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			codeCounts = new Dictionary<ushort, int>();
            originalRandomBetween = MajorityRule.RandomlySelectBetween;
            Assert.IsNotNull(originalRandomBetween);
		}

		//---------------------------------------------------------------------

        private void SetupRandomBetween(int expectedLow,
                                        int expectedHigh,
                                        int randomResult)
        {
            randomBetweenCalled = false;
            this.expectedLow = expectedLow;
            this.expectedHigh = expectedHigh;
            this.randomResult = randomResult;
            MajorityRule.RandomlySelectBetween = RandomBetween;
        }

		//---------------------------------------------------------------------

        private void AssertRandomBetweenCalled()
        {
            Assert.IsTrue(randomBetweenCalled);
        }

		//---------------------------------------------------------------------

        private void RandomBetweenShouldFail()
        {
            MajorityRule.RandomlySelectBetween = RandomBetweenFail;
        }

		//---------------------------------------------------------------------

		[SetUp]
		public void TestInit()
		{
			codeCounts.Clear();
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void NullArgument()
		{
            ushort dummy = MajorityRule.SelectMapCode(null);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentException))]
		public void Count0()
		{
            ushort dummy = MajorityRule.SelectMapCode(codeCounts);
		}

		//---------------------------------------------------------------------

        private void CheckRandomBetweenNotCalled(ushort expectedMapCode)
        {
            RandomBetweenShouldFail();

            ushort selectedMapCode = MajorityRule.SelectMapCode(codeCounts);
            Assert.AreEqual(expectedMapCode, selectedMapCode);
        }

		//---------------------------------------------------------------------

		[Test]
		public void OneCode()
		{
            const ushort expectedMapCode = 123;
            codeCounts[expectedMapCode] = 9;

            CheckRandomBetweenNotCalled(expectedMapCode);
		}

		//---------------------------------------------------------------------

		[Test]
		public void TwoCodes()
		{
            codeCounts[77] = 5;
            codeCounts[11] = 4;
            const ushort expectedMapCode = 77;

            RandomBetweenShouldFail();

            ushort selectedMapCode = MajorityRule.SelectMapCode(codeCounts);
            Assert.AreEqual(expectedMapCode, selectedMapCode);
		}

		//---------------------------------------------------------------------

		public void CheckMostCommonCodes(params ushort[] mostCommonCodes)
		{
            Assert.IsNotNull(mostCommonCodes);
            Assert.IsTrue(mostCommonCodes.Length > 0);

            List<ushort> selectedCodes = new List<ushort>();
            for (int i = 0; i < mostCommonCodes.Length; i++) {
                SetupRandomBetween(0, mostCommonCodes.Length-1, i);
                selectedCodes.Add(MajorityRule.SelectMapCode(codeCounts));
            }
            Assert.That(selectedCodes, Is.EquivalentTo(mostCommonCodes));
		}

		//---------------------------------------------------------------------

		[Test]
		public void TwoCodes_SameCount()
		{
            codeCounts[800] = 2;
            codeCounts[900] = 2;

            CheckMostCommonCodes(800, 900);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ThreeCodes_2MostCommon()
		{
            codeCounts[800] = 4;
            codeCounts[900] = 4;
            codeCounts[100] = 1;

            CheckMostCommonCodes(800, 900);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ThreeCodes_SameCount()
		{
            codeCounts[1] = 3;
            codeCounts[11] = 3;
            codeCounts[111] = 3;

            CheckMostCommonCodes(1, 11, 111);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Block5x5_1MostCommon()
		{
            // 5-by-5 block (25 sites), various distinct codes, 1 most common
            codeCounts[2501] = 5;   // total = 5
            codeCounts[2502] = 3;   // total = 8
            codeCounts[2503] = 1;   // total = 9
            codeCounts[2504] = 1;   // total = 10
            codeCounts[2505] = 2;   // total = 12
            codeCounts[2506] = 2;   // total = 14
            codeCounts[2507] = 2;   // total = 16
            codeCounts[2508] = 2;   // total = 18
            codeCounts[2509] = 4;   // total = 22
            codeCounts[2510] = 3;   // total = 25

            const ushort expectedMapCode = 2501;
            CheckRandomBetweenNotCalled(expectedMapCode);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Block5x5_6MostCommon()
		{
            // 5-by-5 block (25 sites), various distinct codes, 6 most common
            codeCounts[2501] = 4;   // total = 4
            codeCounts[2502] = 4;   // total = 8
            codeCounts[2503] = 4;   // total = 12
            codeCounts[2504] = 1;   // total = 13
            codeCounts[2505] = 4;   // total = 17
            codeCounts[2506] = 4;   // total = 21
            codeCounts[2507] = 4;   // total = 25

            CheckMostCommonCodes(2501, 2502, 2503,   2505, 2506, 2507);
		}

		//---------------------------------------------------------------------

		[TestFixtureTearDown]
		public void TearDown()
		{
            MajorityRule.RandomlySelectBetween = originalRandomBetween;
		}
    }
}
