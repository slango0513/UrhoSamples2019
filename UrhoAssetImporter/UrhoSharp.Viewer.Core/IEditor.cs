using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;

namespace UrhoSharp.Viewer.Core
{
	public interface IEditor
	{
		void HighlightXmlForNode(string node);

		void DisplayModelScale(float scale);

		void DispatchToUI(Action action);

		IConfig Config { get; }
	}
}
