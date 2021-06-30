using BRY;
using Codeplex.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * Fusion360でstlをまとめて書き出し（右クリックでSTL形式で保存）を行うと
 * ファイル名が長くて面倒なのでコンポーネント毎にフォルダ分けするアプリです。
 */ 

namespace Fusion360StlNameComposit
{

	public class StlInfo
	{
		public string Org = "";
		public string Folder = "";
		public string head = "";
		public string[] Comp = new string[0];
		public string  BodyName = "";

		public string ToInfo()
		{
			return Org + "(" +head + "=" + String.Join("=", Comp) + "=" + BodyName+ ")";
		}
		private bool ChkDirectory()
		{
			bool ret = false;
			if (Comp.Length <= 0)
			{
				ret = true;
				return ret;
			}

			string p = Folder;
			ret = true;
			for (int i = 0; i < Comp.Length; i++)
			{
				p = Path.Combine(p, Comp[i]);

				if (System.IO.Directory.Exists(p) == false)
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
		private string newName()
		{
			string ret = Folder;
			if (Comp.Length > 0)
			{
				for (int i = 0; i < Comp.Length; i++)
				{
					ret = Path.Combine(ret, Comp[i]);
				}
			}
			ret = Path.Combine(ret, BodyName + ".stl");
			return ret;
		}
		private string orgName()
		{
			string ret = Folder;
			ret = Path.Combine(ret, Org);
			return ret;
		}
		private bool ChkFileBak(string s)
		{
			if (File.Exists(s) == false) return true;

			string s2 = s + "_";
			int cnt = 10;
			while (cnt >= 0)
			{
				if (File.Exists(s2) == false)
				{
					File.Move(s, s2);
					return true;
				}
				s2 = s2 + "_";
				cnt--;
			}
			return false;
		}
		public bool Rename()
		{
			bool ret = false;

			if (Org == "") return ret;

			if (ChkDirectory() == false) return ret;

			string nn = newName();
			if (ChkFileBak(nn) == false) return ret;
			File.Move(orgName(), nn);
			ret = File.Exists(nn);
			return ret;
		}
	}
	public class StlList :ListBox
	{
		private string m_Folder = "";
		public string Folder
		{
			get { return m_Folder; }
			set
			{
				GetFolder(value);
			}
		}
		private List<StlInfo> m_Infos = new List<StlInfo>();


		private int IndexOfOne(string[] sa, int start)
		{
			int ret = -1;
			if (start >= sa.Length) return ret;
			for ( int i=start;i<sa.Length;i++)
			{
				if (sa[i] == "1")
				{
					ret = i;
					break;
				}
			}
			return ret;
		}
		private int LastIndexOfOne(string[] sa, int start)
		{
			int ret = -1;
			if (start < 0) return ret;
			if (start >= sa.Length) return ret;
			for (int i = start; i >=0; i--)
			{
				if (sa[i] == "1")
				{
					ret = i;
					break;
				}
			}
			return ret;
		}
		public bool GetFolder(string p)
		{
			bool ret = false;

			this.Items.Clear();
			m_Infos.Clear();
			if (File.Exists(p)==true)
			{
				if (Path.GetExtension(p).ToLower()==".stl")
				{
					p = Path.GetDirectoryName(p);
				}
				else
				{
					return ret;
				}
			}
			if (Directory.Exists(p) == false) return ret;
			List<string> files = new List<string>();
			foreach (string file in Directory.EnumerateFiles(p))
			{
				if (Path.GetExtension(file).ToLower()==".stl")
				{
					files.Add(Path.GetFileNameWithoutExtension(file));
				}
			}
			if (files.Count <= 1) return ret;


			string s0 = files[0];
			string s1 = files[files.Count - 1];
			int cnt = s0.Length;
			if (cnt > s1.Length) cnt = s1.Length;
			int idx = -1;
			for(int i=0; i<cnt;i++)
			{
				if (s0[i]!= s1[i])
				{
					idx = i;
					break;
				}
			}
			if (idx <= -1) return ret;

			string head = s0.Substring(0, idx);

			foreach(string sf in files)
			{
				string [] sa = sf.Substring(idx).Split('_');
				if (sa.Length <= 1) continue;

				Array.Resize(ref sa, sa.Length - 1);

				StlInfo si = new StlInfo();
				si.Org = sf+".stl";
				si.head = head;
				si.Folder = p;
				if (sa.Length==1)
				{
					si.BodyName = sa[0];
				}
				else
				{
					int index = 0;
					int last = sa.Length;
					List<string> cp = new List<string>();
					while(index < last)
					{
						int v = IndexOfOne(sa, index);
						string bn = "";
						if (v < 0)
						{
							int last2 = last;
							if (cp.Count>0)
							{
								string [] cpa = cp[cp.Count - 1].Split('_');
								if (cpa.Length>1)
								{
									last2 -= (cpa.Length - 1);
								}
							}

							bn = "";
							for (int j= index; j < last2; j++)
							{
								if (bn != "") bn += "_";
								bn += sa[j];
							}
							si.BodyName = bn;
							break;
						}
						else
						{
							bn = "";
							for (int j = index; j < v; j++)
							{
								if (bn != "") bn += "_";
								bn += sa[j];
							}
							cp.Add(bn);
							index = v + 1;
						}
						if (index >= last) break;
					}
					if (cp.Count > 0) si.Comp = cp.ToArray();
				}
				m_Infos.Add(si);
			}

			foreach(StlInfo ss in m_Infos)
			{
				this.Items.Add(ss.ToInfo());
			}
			m_Folder = p;
			ret = true;

			return ret;
		}

		public bool Rename()
		{
			bool ret = false;
			if (m_Infos.Count <= 0) return ret;

			ret = true;
			foreach (StlInfo si in m_Infos)
			{
				if ( si.Rename()==false)
				{
					ret = false;
				}
			}
			GetFolder(m_Folder);
			return ret;
		}
	}
}
