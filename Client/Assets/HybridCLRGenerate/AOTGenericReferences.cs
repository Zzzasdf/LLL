using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"IDataService.dll",
		"Microsoft.Extensions.DependencyInjection.Abstractions.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// System.Func<object,object>
	// }}

	public void RefMethods()
	{
		// object IDataService.Get<object>()
		// object Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetService<object>(System.IServiceProvider)
	}
}