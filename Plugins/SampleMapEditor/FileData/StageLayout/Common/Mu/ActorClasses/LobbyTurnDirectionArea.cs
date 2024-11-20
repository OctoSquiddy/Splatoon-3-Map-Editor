using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LobbyTurnDirectionArea : MuObj
	{
		[BindGUI("SafeDir", Category = "LobbyTurnDirectionArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public OpenTK.Vector3 _SafeDir
		{
			get
			{
				return new OpenTK.Vector3(
					this.spl__LobbyTurnDirectionAreaBancParam.SafeDir.X,
					this.spl__LobbyTurnDirectionAreaBancParam.SafeDir.Y,
					this.spl__LobbyTurnDirectionAreaBancParam.SafeDir.Z);
			}

			set
			{
				this.spl__LobbyTurnDirectionAreaBancParam.SafeDir = new ByamlVector3F(value.X, value.Y, value.Z);
			}
		}

		[ByamlMember("spl__LobbyTurnDirectionAreaBancParam")]
		public Mu_spl__LobbyTurnDirectionAreaBancParam spl__LobbyTurnDirectionAreaBancParam { get; set; }

		public LobbyTurnDirectionArea() : base()
		{
			spl__LobbyTurnDirectionAreaBancParam = new Mu_spl__LobbyTurnDirectionAreaBancParam();

			Links = new List<Link>();
		}

		public LobbyTurnDirectionArea(LobbyTurnDirectionArea other) : base(other)
		{
			spl__LobbyTurnDirectionAreaBancParam = other.spl__LobbyTurnDirectionAreaBancParam.Clone();
		}

		public override LobbyTurnDirectionArea Clone()
		{
			return new LobbyTurnDirectionArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LobbyTurnDirectionAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
