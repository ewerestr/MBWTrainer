using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Windows.Forms;

namespace MBWTrainer
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpbuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr pHandle, IntPtr Address, byte[] Buffer, int Size, IntPtr NumberofBytesRead);
        private int[] offsets = { 0x5D0, 0x2C8, 0x270, 0x274, 0x278, 0x27C, 0x2B4, 0x2BC, 0x2C0, 0x2C4 };
        private IntPtr[] addrs;
        private int[] vls = new int[10];
        string PROCESS_NAME = "mb_warband";
        IntPtr MAIN_OFFSET = new IntPtr(0x45DF24);
        IntPtr SECONT_OFFSET = new IntPtr(0x140DC);
        IntPtr base_addr;
        
        int pid;
        IntPtr handle;

        public Form1()
        {
            InitializeComponent();
            getProcess();
            calculateAddresses();
            updateData();
        }

        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }
        private void updateData()
        {
            byte[] tm = new byte[4];
            IntPtr q = new IntPtr();
            ReadProcessMemory(handle, addrs[1], tm, 4, q);
            vls[1] = BitConverter.ToInt32(tm, 0);
            tb_level.Text = vls[1].ToString(); tm = new byte[4];
            ReadProcessMemory(handle, addrs[0], tm, 4, q);
            vls[0] = BitConverter.ToInt32(tm, 0);
            tb_money.Text = vls[0].ToString(); tm = new byte[4];
            ReadProcessMemory(handle, addrs[6], tm, 4, q);
            vls[6] = BitConverter.ToInt32(tm, 0);
            tb_experience.Text = vls[6].ToString(); tm = new byte[4];
            ReadProcessMemory(handle, addrs[2], tm, 4, q);
            vls[2] = BitConverter.ToInt32(tm, 0);
            tb_strength.Text = vls[2].ToString(); tm = new byte[4];
            ReadProcessMemory(handle, addrs[3], tm, 4, q);
            vls[3] = BitConverter.ToInt32(tm, 0);
            tb_adroitness.Text = vls[3].ToString(); tm = new byte[4];
            ReadProcessMemory(handle, addrs[4], tm, 4, q);
            vls[4] = BitConverter.ToInt32(tm, 0);
            tb_intelligence.Text = vls[4].ToString(); tm = new byte[4];
            ReadProcessMemory(handle, addrs[5], tm, 4, q);
            vls[5] = BitConverter.ToInt32(tm, 0);
            tb_charisma.Text = vls[5].ToString(); tm = new byte[4];
            ReadProcessMemory(handle, addrs[8], tm, 4, q);
            vls[8] = BitConverter.ToInt32(tm, 0);
            tb_characteristics.Text = vls[8].ToString(); tm = new byte[4];
            ReadProcessMemory(handle, addrs[7], tm, 4, q);
            vls[7] = BitConverter.ToInt32(tm, 0);
            tb_skills.Text = vls[7].ToString(); tm = new byte[4];
            ReadProcessMemory(handle, addrs[9], tm, 4, q);
            vls[9] = BitConverter.ToInt32(tm, 0);
            tb_weaponskills.Text = vls[9].ToString();
        }
        private void getProcess()
        {
            Process[] pls = Process.GetProcesses();
            if (pls.Count() > 0)
            {
                foreach (Process process in pls)
                {
                    if (process.ProcessName == PROCESS_NAME)
                    {
                        prclbl.Text = process.MainModule.ModuleName.ToString();
                        pid = process.Id;
                        pidholderlbl.Text = pid.ToString();
                        base_addr = process.MainModule.BaseAddress;
                        baddrlbl.Text = "0x" + base_addr.ToString("X8");
                        handle = OpenProcess(ProcessAccessFlags.All, false, pid);
                        break;
                    }
                }
            }
        }
        private void calculateAddresses()
        {
            addrs = new IntPtr[10];
            IntPtr handle = OpenProcess(ProcessAccessFlags.All, false, pid);
            byte[] t = new byte[4];
            IntPtr q = new IntPtr();
            IntPtr w = new IntPtr(base_addr.ToInt32() + MAIN_OFFSET.ToInt32());
            ReadProcessMemory(handle, w, t, 4, q);
            int db1 = BitConverter.ToInt32(t, 0);
            w = new IntPtr(BitConverter.ToInt32(t, 0) + SECONT_OFFSET.ToInt32()); t = new byte[4];
            ReadProcessMemory(handle, w, t, 4, q);
            int db2 = BitConverter.ToInt32(t, 0);
            w = new IntPtr(BitConverter.ToInt32(t, 0)); t = new byte[4];
            for (int a = 0; a < offsets.Length; a++) addrs[a] = IntPtr.Add(w, offsets[a]);
            CloseHandle(handle);
        }
        private void WriteBytes(IntPtr address, byte[] hexval)
        {
            if (pid != 0 && address.ToInt32() != 0)
            {
                IntPtr handle = OpenProcess(ProcessAccessFlags.All, false, pid);
                UIntPtr q = new UIntPtr();
                WriteProcessMemory(handle, address, hexval, 4, out q);
                CloseHandle(handle);
            }
        }

        private void llb_con_0_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://vk.com/id258724542");
        }

        private void tb_level_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if(int.TryParse(tb_level.Text, out a) && (a != vls[1] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[1], BitConverter.GetBytes(a));
            }
        }

        private void tb_money_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if (int.TryParse(tb_money.Text, out a) && (a != vls[0] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[0], BitConverter.GetBytes(a));
            }
        }

        private void tb_experience_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if (int.TryParse(tb_experience.Text, out a) && (a != vls[6] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[6], BitConverter.GetBytes(a));
            }
        }

        private void tb_strength_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if (int.TryParse(tb_strength.Text, out a) && (a != vls[2] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[2], BitConverter.GetBytes(a));
            }
        }

        private void tb_adroitness_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if (int.TryParse(tb_adroitness.Text, out a) && (a != vls[3] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[3], BitConverter.GetBytes(a));
            }
        }

        private void tb_intelligence_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if (int.TryParse(tb_intelligence.Text, out a) && (a != vls[4] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[4], BitConverter.GetBytes(a));
            }
        }

        private void tb_charisma_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if (int.TryParse(tb_charisma.Text, out a) && (a != vls[5] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[5], BitConverter.GetBytes(a));
            }
        }

        private void tb_characteristics_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if (int.TryParse(tb_characteristics.Text, out a) && (a != vls[8] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[8], BitConverter.GetBytes(a));
            }
        }

        private void tb_skills_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if (int.TryParse(tb_skills.Text, out a) && (a != vls[7] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[7], BitConverter.GetBytes(a));
            }
        }

        private void tb_weaponskills_KeyDown(object sender, KeyEventArgs e)
        {
            int a;
            if (int.TryParse(tb_weaponskills.Text, out a) && (a != vls[9] && a >= 0) && e.KeyCode == Keys.Enter)
            {
                WriteBytes(addrs[9], BitConverter.GetBytes(a));
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseHandle(handle);
        }

        private void bt_level_ok_Click(object sender, EventArgs e)
        {
            int a;
            if(int.TryParse(tb_level.Text, out a) && (a != vls[1] && a >= 0)) WriteBytes(addrs[1], BitConverter.GetBytes(a));
        }

        private void bt_money_ok_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(tb_money.Text, out a) && (a != vls[0] && a >= 0)) WriteBytes(addrs[0], BitConverter.GetBytes(a));
        }

        private void bt_experience_ok_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(tb_experience.Text, out a) && (a != vls[6] && a >= 0)) WriteBytes(addrs[6], BitConverter.GetBytes(a));
        }

        private void bt_strength_ok_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(tb_strength.Text, out a) && (a != vls[2] && a >= 0)) WriteBytes(addrs[2], BitConverter.GetBytes(a));
        }

        private void bt_adroitness_ok_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(tb_adroitness.Text, out a) && (a != vls[3] && a >= 0)) WriteBytes(addrs[3], BitConverter.GetBytes(a));
        }

        private void bt_intelligence_ok_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(tb_intelligence.Text, out a) && (a != vls[4] && a >= 0)) WriteBytes(addrs[4], BitConverter.GetBytes(a));
        }

        private void bt_charisma_ok_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(tb_charisma.Text, out a) && (a != vls[5] && a >= 0)) WriteBytes(addrs[5], BitConverter.GetBytes(a));
        }

        private void bt_characteristics_ok_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(tb_characteristics.Text, out a) && (a != vls[8] && a >= 0)) WriteBytes(addrs[8], BitConverter.GetBytes(a));
        }

        private void bt_skills_ok_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(tb_skills.Text, out a) && (a != vls[7] && a >= 0)) WriteBytes(addrs[7], BitConverter.GetBytes(a));
        }

        private void bt_weaponskills_ok_Click(object sender, EventArgs e)
        {
            int a;
            if (int.TryParse(tb_weaponskills.Text, out a) && (a != vls[9] && a >= 0)) WriteBytes(addrs[9], BitConverter.GetBytes(a));
        }
    }
}
