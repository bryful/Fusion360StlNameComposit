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

/// <summary>
/// 基本となるアプリのスケルトン
/// </summary>
namespace Fusion360StlNameComposit
{
	public partial class Form1 : Form
	{
		string m_folder = "";
		//-------------------------------------------------------------
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Form1()
		{
			InitializeComponent();
		}
		/// <summary>
		/// コントロールの初期化はこっちでやる
		/// </summary>
		protected override void InitLayout()
		{
			base.InitLayout();
		}
		//-------------------------------------------------------------
		/// <summary>
		/// フォーム作成時に呼ばれる
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender, EventArgs e)
		{
			//設定ファイルの読み込み
			JsonPref pref = new JsonPref();
			if (pref.Load())
			{
				bool ok = false;
				Size sz = pref.GetSize("Size", out ok);
				if (ok) this.Size = sz;
				Point p = pref.GetPoint("Point", out ok);
				if (ok) this.Location = p;
				string f = pref.GetString("Folder" ,out ok);
				if (ok) m_folder = f;
			}
			this.Text = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
		}
		//-------------------------------------------------------------
		/// <summary>
		/// フォームが閉じられた時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			//設定ファイルの保存
			JsonPref pref = new JsonPref();
			pref.SetSize("Size", this.Size);
			pref.SetPoint("Point", this.Location);

			pref.SetString("Folder", m_folder);
			pref.Save();

		}
		//-------------------------------------------------------------
		/// <summary>
		/// ドラッグ＆ドロップの準備
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}
		/// <summary>
		/// ドラッグ＆ドロップの本体
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			//ここでは単純にファイルをリストアップするだけ
			GetCommand(files);
		}
		//-------------------------------------------------------------
		/// <summary>
		/// ダミー関数
		/// </summary>
		/// <param name="cmd"></param>
		public void GetCommand(string[] cmd)
		{
			if (cmd.Length > 0)
			{
				foreach (string s in cmd)
				{
					if (GetFolder(s)==true)
					{
						break;
					}
				}
			}
		}
		/// <summary>
		/// メニューの終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//-------------------------------------------------------------
		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AppInfoDialog.ShowAppInfoDialog();
		}
		public bool GetFolder(string p)
		{
			bool ret = false;
			BtnExec.Enabled = false;
			ret = stlList1.GetFolder(p);
			if (stlList1.Items.Count>0)
			{
				BtnExec.Enabled = true;
			}
			return ret;
		}
		public bool Exec()
		{
			bool ret = false;
			if (stlList1.Items.Count > 0)
			{
				ret = stlList1.Rename();
			}
			BtnExec.Enabled = (stlList1.Items.Count > 0);

			return ret;
		}
		public bool SelectFolder()
		{
			bool ret = false;

			using (var ofd = new OpenFileDialog() { FileName = "　", Filter = "Folder|.", CheckFileExists = false })
			{
				ofd.FileName = "フォルダを選択";
				if( Directory.Exists( m_folder)==true)
				{
					ofd.InitialDirectory = m_folder;
				}
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					string p = Path.GetDirectoryName(ofd.FileName);
					ret = GetFolder(p);
					if(ret)
					{
						m_folder = p;
					}

				}
			}
			return ret;
		}

		private void SelectFolderMenu_Click(object sender, EventArgs e)
		{
			SelectFolder();
		}

		private void BtnExec_Click(object sender, EventArgs e)
		{
			Exec();
		}

		private void execMenu_Click(object sender, EventArgs e)
		{
			Exec();
		}
	}
}
