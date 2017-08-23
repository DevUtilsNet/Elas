namespace DevUtils.Elas.Tasks.Core.Diagnostics
{
	sealed class ElasTraceSourceCore : ElasTraceSourceBase
	{
		private static ElasTraceSourceCore _instance;

		public static ElasTraceSourceCore Instance
		{
			get { return _instance ?? (_instance = new ElasTraceSourceCore()); }
		}

		private ElasTraceSourceCore()
			: base("Elase.Core")
		{
			
		}
	}
}