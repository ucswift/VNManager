using StructureMap;

namespace WaveTech.VNManager.Common
{
	public static class ObjectLocator
	{
		private static bool IsInitialized;

		private static void Initialize()
		{
			if (!IsInitialized)
			{
				Bootstrapper.Configure();
				IsInitialized = true;
			}
		}

		public static T GetInstance<T>()
		{
			Initialize();
			return ObjectFactory.GetInstance<T>();
		}

		public static T GetInstance<T>(string name)
		{
			Initialize();
			return ObjectFactory.GetNamedInstance<T>(name);
		}
	}
}