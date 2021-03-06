﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CourseProject.Dictionary;

using NUnit.Framework;

namespace CourseProject.Tests
{
    [TestFixture]
    internal sealed class CompositeDictionaryTests
    {
        [Test]
        public void IndexerAddTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>();
            compositeDictionary["Id", "Name"] = 1;

            Assert.That(compositeDictionary["Id", "Name"], Is.EqualTo(1));
        }

        [Test]
        public void IndexerSetTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
                                          {
                                              { "Id", "Name", 1 }
                                          };
            compositeDictionary["Id", "Name"] = 2;

            Assert.That(compositeDictionary["Id", "Name"], Is.EqualTo(2));
        }

        [Test]
        public void GetByIdTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
                                          {
                                              { "Id", "Name1", 1 },
                                              { "Id", "Name2", 2 },
                                              { "Id2", "Name3", 3 }
                                          };
            var expectedValues = new List<int>
                                     {
                                         1,
                                         2
                                     };

            Assert.That(compositeDictionary.GetById("Id"), Is.EqualTo(expectedValues));
        }

        [Test]
        public void GetByNameTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
                                          {
                                              { "Id1", "Name", 1 },
                                              { "Id2", "Name", 2 },
                                              { "Id3", "Name1", 3 }
                                          };
            var expectedValues = new List<int>
                                     {
                                         1,
                                         2
                                     };

            Assert.That(compositeDictionary.GetByName("Name"), Is.EqualTo(expectedValues));
        }

        [Test]
        public void CountTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
                                          {
                                              { "Id1", "Name1", 1 },
                                              { "Id1", "Name2", 2 },
                                              { "Id2", "Name1", 3 },
                                              { "Id2", "Name2", 4 },
                                              { "Id3", "Name1", 5 }
                                          };
            Assert.That(compositeDictionary.Count, Is.EqualTo(5));
        }

        [Test]
        public void EnumirableTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
                                          {
                                              { "Id1", "Name1", 1 },
                                              { "Id1", "Name2", 2 },
                                              { "Id2", "Name1", 3 },
                                              { "Id2", "Name2", 4 },
                                              { "Id3", "Name1", 5 }
                                          };

            var iterationsNumber = 0;

            foreach (var value in compositeDictionary)
            {
                iterationsNumber++;
            }

            Assert.That(iterationsNumber, Is.EqualTo(5));
        }

        [Test]
        public void RemoveTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
                                          {
                                              { "Id1", "Name1", 1 },
                                              { "Id1", "Name2", 2 },
                                              { "Id2", "Name1", 3 },
                                              { "Id2", "Name2", 4 },
                                              { "Id3", "Name1", 5 }
                                          };
            var result = compositeDictionary.Remove("Id2", "Name2");

            Assert.That(result, Is.True);
            Assert.That(compositeDictionary.Count, Is.EqualTo(4));
            Assert.That(!compositeDictionary.Contains(new KeyValuePair<CompositeKey<string, string>, int>(new CompositeKey<string, string>("Id2", "Name2"), 4)));
        }

        [Test]
        public void RemoveItemTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
            {
                { "Id1", "Name1", 1 },
                { "Id1", "Name2", 2 },
                { "Id2", "Name1", 3 },
                { "Id2", "Name2", 4 },
                { "Id3", "Name1", 5 }
            };
            var result = compositeDictionary.Remove(new KeyValuePair<CompositeKey<string, string>, int>(new CompositeKey<string, string>("Id2", "Name2"), 4));

            Assert.That(result, Is.True);
            Assert.That(compositeDictionary.Count, Is.EqualTo(4));
            Assert.That(!compositeDictionary.Contains(new KeyValuePair<CompositeKey<string, string>, int>(new CompositeKey<string, string>("Id2", "Name2"), 4)));
        }

        [Test]
        public void ClearTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
                                          {
                                              { "Id1", "Name1", 1 },
                                              { "Id1", "Name2", 2 },
                                              { "Id2", "Name1", 3 },
                                              { "Id2", "Name2", 4 },
                                              { "Id3", "Name1", 5 }
                                          };
            compositeDictionary.Clear();

            Assert.That(compositeDictionary.Count, Is.EqualTo(0));
        }

        [Test]
        public void DuplicatesIdTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
            {
                { "Id1", "Name1", 1 },
                { "Id1", "Name2", 2 },
            };

            var initialCompositeDictionaryCount = compositeDictionary.Count;

            var isExceptionThrown = false;
            try
            {
                compositeDictionary.Add("Id1", "Name1", 3);
            }
            catch (ArgumentException e)
            {
                isExceptionThrown = true;
            }

            Assert.That(isExceptionThrown, Is.EqualTo(true), "ArgumentException didn't throw.");
            Assert.That(compositeDictionary.Count, Is.EqualTo(initialCompositeDictionaryCount));
        }

        [Test]
        public void ThreadsTest()
        {
            var compositeDictionary = new CompositeDictionary<string, string, int>
            {
                {"Id1", "Name1", 1},
                {"Id1", "Name2", 2},
                {"Id2", "Name1", 3},
                {"Id2", "Name2", 4},
                {"Id3", "Name1", 5}
            };

            var initCount = compositeDictionary.Count;

            Task.Run(() => compositeDictionary.Add("TId1", "TName1", 6));
            Task.Run(() => compositeDictionary.Add("TId2", "TName2", 7));
            Task.Run(() => compositeDictionary.Add("TId3", "TName3", 8));

            Thread.Sleep(10);

            Assert.AreEqual(initCount + 3, compositeDictionary.Count);
        }
    }
}
