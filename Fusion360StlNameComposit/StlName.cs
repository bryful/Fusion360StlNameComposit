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
		public string ParentDirectory { get { return m_Directory; } }
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

			string n = Path.GetFileNameWithoutExtension(s);

			string[] na = n.Split('_');
			if (na.Length < 4) return ret;


			// 最後のコンポーネント名を消す。
			// とりあえずあったらEmptyにしておく
			/*
			if ((na[na.Length-1]!="")&&(na.Length>=4))
			{
				if (na[na.Length-1] == na[na.Length - 4])
				{
					na[na.Length - 1] = "";
				}
			}*/
			Array.Resize(ref na, na.Length - 1);

			m_Org = s;
			m_ExportName = na[0];
			m_DocName = na[1];
			m_BodyName = na[na.Length - 1];
			m_Directory = Path.GetDirectoryName(s);
			m_Ext = Path.GetExtension(s);

			int st = 2;
			int ed = na.Length - 1;
			//コンポーネントがあるか？
			if (ed>st)
			{
				int idx = st;
				int cnt = ed - st;
				m_comps = new string[cnt / 2];

				int idx2 = 0;
				while(idx<na.Length)
				{
					m_comps[idx2] = na[idx];
					idx++;
					idx++;
					if (idx >= na.Length) break;
					idx2++;
					if (idx2 >= m_comps.Length) break;
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
		public bool Rename()
		{
			bool ret = false;

			if (m_Org == "") return ret;

			if (ChkDirectory() == false) return ret;

			string nn = newName();
			if (ChkFileBak(nn) == false) return ret;
			File.Move(m_Org, nn);
			ret = File.Exists(nn);
			return ret;
		}
		private bool ChkFileBak(string s)
		{
			if (File.Exists(s) == false) return true;

			string s2 = s + "_";
			int cnt = 10;
			while(cnt>=0)
			{
				if (File.Exists(s2)==false)
				{
					File.Move(s, s2);
					return true;
				}
				s2 = s2 + "_";
				cnt--;
			}
			return false;
		}
		private string newName()
		{
			string ret = m_Directory;
			if (m_comps.Length>0)
			{
				for ( int i=0; i<m_comps.Length;i++)
				{
					ret = Path.Combine(ret, m_comps[i]);
				}
			}
			ret = Path.Combine(ret, m_BodyName+m_Ext);

			return ret;
		}
		private bool ChkDirectory()
		{
			bool ret = false;
			if (m_comps.Length <= 0)
			{
				ret = true;
				return ret;
			}

			string p = m_Directory;
			ret = true;
			for (int i=0; i<m_comps.Length;i++)
			{
				p = Path.Combine(p, m_comps[i]);
				
				if( System.IO.Directory.Exists(p)==false)
				{
					System.IO.Directory.CreateDirectory(p);
					if (System.IO.Directory.Exists(p) == false)
					{
						ret = false;
						break;
					}
				}
			}

			return ret;
		}
	}
}
