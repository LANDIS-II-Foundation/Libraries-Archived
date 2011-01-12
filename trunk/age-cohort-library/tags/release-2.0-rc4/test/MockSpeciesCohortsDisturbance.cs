using Landis.AgeCohort;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;

namespace Landis.Test.AgeCohort
{
    public class MockSpeciesCohortsDisturbance
        : ISpeciesCohortsDisturbance
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:test");

        //---------------------------------------------------------------------

        PlugInType IDisturbance.Type
        {
            get {
                return MockSpeciesCohortsDisturbance.Type;
            }
        }

        //---------------------------------------------------------------------

        public ActiveSite CurrentSite;

        ActiveSite IDisturbance.CurrentSite
        {
            get {
                return CurrentSite;
            }
        }

        //---------------------------------------------------------------------

        public delegate void DamageMethod(ISpeciesCohorts         cohorts,
                                          ISpeciesCohortBoolArray isDamaged);

        public DamageMethod Damage;
        public ISpecies SelectedSpecies;

        //---------------------------------------------------------------------

        void ISpeciesCohortsDisturbance.Damage(ISpeciesCohorts         cohorts,
                                               ISpeciesCohortBoolArray isDamaged)
        {
            Damage(cohorts, isDamaged);
        }

        //---------------------------------------------------------------------

        public void ClearCut(ISpeciesCohorts         cohorts,
                             ISpeciesCohortBoolArray isDamaged)
        {
            for (int i = 0; i < isDamaged.Count; i++)
                isDamaged[i] = true;
        }

        //---------------------------------------------------------------------

        public void AllOfSelectedSpecies(ISpeciesCohorts         cohorts,
                                         ISpeciesCohortBoolArray isDamaged)
        {
            if (cohorts.Species == SelectedSpecies) {
                for (int i = 0; i < isDamaged.Count; i++)
                    isDamaged[i] = true;
            }
        }

        //---------------------------------------------------------------------

        public void OldestOfSelectedSpecies(ISpeciesCohorts         cohorts,
                                            ISpeciesCohortBoolArray isDamaged)
        {
            if (cohorts.Species == SelectedSpecies)
                //  Oldest is first cohort
                isDamaged[0] = true;
        }

        //---------------------------------------------------------------------

        public void AllExceptYoungest(ISpeciesCohorts         cohorts,
                                      ISpeciesCohortBoolArray isDamaged)
        {
            //  Youngest is the last cohort (at index Count - 1)
            for (int i = 0; i < (isDamaged.Count - 1); i++)
                isDamaged[i] = true;
        }

        //---------------------------------------------------------------------

        public void Every2ndCohort(ISpeciesCohorts         cohorts,
                                   ISpeciesCohortBoolArray isDamaged)
        {
            int N = 2;
            //  Every Nth cohort, working from youngest to oldest
            int youngestIndex = isDamaged.Count - 1;
            for (int i = youngestIndex - (N - 1); i >= 0; i -= N)
                isDamaged[i] = true;
        }
    }
}
