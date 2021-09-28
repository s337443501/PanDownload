using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PanDownload
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            int processCount = 0;
            Process[] process = Process.GetProcesses();
            foreach (Process PTest in process)
            {
                if (PTest.ProcessName == Process.GetCurrentProcess().ProcessName)
                {
                    processCount += 1;
                }
            }
            if (processCount > 1)
            {
                DialogResult dr = MessageBox.Show("程序正在运行，您可以双击托盘菜单中的图标打开！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
