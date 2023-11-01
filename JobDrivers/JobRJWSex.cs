    public class RJWSexJob : Job, IExposable
    {
        public VorePath VorePath;
        public VoreProposal Proposal;
        public Pawn Initiator;
        public bool IsForced = false;
        public bool IsKidnapping = false;
        public bool IsRitualRelated = false;

        // we hide base ExposeData here because Tynan thought it was a great idea to completely lock and seal the Job. This entire file is just a mess of workarounds for the restrictive Job class
        public new void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref VorePath, "VorePath", new object[0]);
            Scribe_Deep.Look(ref Proposal, "Proposal", new object[0]);
            Scribe_References.Look(ref Initiator, "Initiator");
            Scribe_Values.Look(ref IsKidnapping, "IsKidnapping");
        }
    }

    public static class VoreJobMaker
    {
        public static VoreJob MakeJob()
        {
            VoreJob job = SimplePool<VoreJob>.Get();
            job.loadID = Find.UniqueIDsManager.GetNextJobID();
            return job;
        }
        public static VoreJob MakeJob(JobDef jobDef)
        {
            VoreJob job = MakeJob();
            job.def = jobDef;
            return job;
        }
        public static VoreJob MakeJob(JobDef jobDef, LocalTargetInfo targetA)
        {
            VoreJob job = MakeJob();
            job.def = jobDef;
            job.targetA = targetA;
... -> more MakeJob() overloads with targetA, targetB, etc. but msg too long
    }