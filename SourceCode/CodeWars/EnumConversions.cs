using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeWars
{
    public enum AdellijkeTitelPredikaat
    {

        /// <remarks/>
        Baron,

        /// <remarks/>
        Barones,

        /// <remarks/>
        Graaf,

        /// <remarks/>
        Gravin,

        /// <remarks/>
        Hertog,

        /// <remarks/>
        Hertogin,

        /// <remarks/>
        Jonkheer,

        /// <remarks/>
        Jonkvrouw,

        /// <remarks/>
        Markies,

        /// <remarks/>
        Markiezin,

        /// <remarks/>
        Prins,

        /// <remarks/>
        Prinses,

        /// <remarks/>
        Ridder,
    }

    [TestFixture]
    public class EnumConversions
    {

        public static AdellijkeTitelPredikaat? ToAdellijkeTitelPredikaat(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return
                (AdellijkeTitelPredikaat?)
                (Enum.TryParse(typeof(AdellijkeTitelPredikaat), value, true, out var result)
                ? result
                : null);
        }

        private static AdellijkeTitelPredikaat? GetAdellijkeTitelPredikaat(string value)
        {
            if (value == null) return null;

            if (value.Equals("Baron")) return AdellijkeTitelPredikaat.Baron;
            if (value.Equals("Barones")) return AdellijkeTitelPredikaat.Barones;
            if (value.Equals("Graaf")) return AdellijkeTitelPredikaat.Graaf;
            if (value.Equals("Gravin")) return AdellijkeTitelPredikaat.Gravin;
            if (value.Equals("Hertog")) return AdellijkeTitelPredikaat.Hertog;
            if (value.Equals("Hertogin")) return AdellijkeTitelPredikaat.Hertogin;
            if (value.Equals("Jonkheer")) return AdellijkeTitelPredikaat.Jonkheer;
            if (value.Equals("Jonkvrouw")) return AdellijkeTitelPredikaat.Jonkvrouw;
            if (value.Equals("Markies")) return AdellijkeTitelPredikaat.Markies;
            if (value.Equals("Markiezin")) return AdellijkeTitelPredikaat.Markiezin;
            if (value.Equals("Prins")) return AdellijkeTitelPredikaat.Prins;
            if (value.Equals("Prinses")) return AdellijkeTitelPredikaat.Prinses;
            if (value.Equals("Ridder")) return AdellijkeTitelPredikaat.Ridder;
            return null;
        }

        [TestCase("Baron", AdellijkeTitelPredikaat.Baron)]
        [TestCase("", null)]
        [TestCase(null, null)]
        [TestCase("BARON", AdellijkeTitelPredikaat.Baron)]
        [TestCase("JONKHEER", AdellijkeTitelPredikaat.Jonkheer)]
        [TestCase("Jonkheer", AdellijkeTitelPredikaat.Jonkheer)]
        public void GetAdellijkeTitelPredikaatTest(string value, AdellijkeTitelPredikaat? expected)
        {
            Assert.AreEqual(expected, GetAdellijkeTitelPredikaat(value));
        }

        [TestCase("Baron", AdellijkeTitelPredikaat.Baron)]
        [TestCase("", null)]
        [TestCase(null, null)]
        [TestCase("BARON", AdellijkeTitelPredikaat.Baron)]
        [TestCase("JONKHEER", AdellijkeTitelPredikaat.Jonkheer)]
        [TestCase("Jonkheer", AdellijkeTitelPredikaat.Jonkheer)]
        public void ToAdellijkeTitelPredikaatTest(string value, AdellijkeTitelPredikaat? expected)
        {
            Assert.AreEqual(expected, ToAdellijkeTitelPredikaat(value));
        }
    }
}
