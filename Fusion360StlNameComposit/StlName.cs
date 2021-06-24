using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Fusion360StlNameComposit
{
	public class StlName
	{
		private string m_Org = "";
		public string Org { get { return m_Org; } }
		private string m_Directory = "";
		public string Directory { get { return m_Directory; } }
		private string m_Ext = "";
		public string Ext { get { return m_Ext; } }
		private string m_BodyName = "";
		public string BodyName { get { return m_BodyName; } }
		private string[] m_comps = new string[0];
		private string[] Comps  { get { return m_comps; } }
		private string m_ExportName = "";
		public string ExportName { get { return m_ExportName; } }
		private string m_DocName = "";
		public string DocName { get { return m_DocName; } }

		public StlName()
		{
			Clear();
		}
		public StlName(string n)
		{
			SetSltName(n);
		}
		public void Clear()
		{
			m_Org = "";
			m_Directory = "";
			m_Ext = "";
			m_BodyName = "";
			m_comps = new string[0];
			m_ExportName = "";
			m_DocName = "";
		}
		public bool SetSltName(string s)
		{
			Clear();
			bool ret = false;
			if (s == "") return ret;

			string d = Path.GetDirectoryName(s);
			string e = Path.GetExtension(s);
			string n = Path.GetFileNameWithoutExtension(s);

			string[] na = n.Split('_');
			if (na.Length <= 4) return ret;

			// 最後のコンポーネント名を消す。
			// とりあえずあったらEmptyにしておく
			if ((na[na.Length-1]!="")&&(na.Length>=4))
			{
				if (na[na.Length-1] == na[na.Length - 4])
				{
					na[na.Length - 1] = "";
				}
			}
			m_Org = s;
			m_ExportName = na[0];
			m_DocName = na[1];
			m_BodyName = na[na.Length - 2];

			int st = 2;
			int ed = na.Length - 2;
			//コンポーネントがあるか？
			if (ed>st)
			{
				int idx = 0;
				int cnt = ed - st;
				m_comps = new string[cnt / 2];

				int idx2 = 0;
				while(idx<cnt)
				{
					m_comps[idx2] = na[idx + st];
					idx+=2;
					idx2++;
				}
			}
			return ret;
		}
		public string ToInfo()
		{
			string ret = "";
			ret += "Body:" + m_BodyName + "\r\n";
			ret += "Comps: [" + String.Join(",",m_comps)+"]\r\n";
			return ret;
		}
	}
}
