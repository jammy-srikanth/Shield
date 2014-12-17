using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace Registration
{
    class Program
    {
        private static bool IsBackup = false;
        static Microsoft.Win32.RegistryKey key1;
        static Microsoft.Win32.RegistryKey key2;
        public static void Main(string[] args)
        {
            try
            {
                Program program = new Program();
                key1 = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("sip");
                key2 = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("sip.backup");
                if (key2 == null)
                {
                    key2 = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("sip.backup");
                    program.copyTo(key1, key2);
                    IsBackup = true;
                    program.EnableRegistry();
                    key1.Close();
                    key2.Close();
                }
                else
                {
                    IsBackup = true;
                    program.EnableRegistry();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration.Main() :\n" + ex.Message, "Exception in Registration");
            }
            return;
        }
        //copyTo method will copy the key from src to dst , essentially to take backup for sip to sip.backup
        public void copyTo(Microsoft.Win32.RegistryKey src, Microsoft.Win32.RegistryKey dest)
        {
            try
            {
                foreach (String name in src.GetValueNames())
                {
                    dest.SetValue(name, src.GetValue(name), src.GetValueKind(name));
                }
                // copy the subkeys
                foreach (String name in src.GetSubKeyNames())
                {
                    using (Microsoft.Win32.RegistryKey srcSubKey = src.OpenSubKey(name, false))
                    {
                        Microsoft.Win32.RegistryKey dstSubKey = dest.CreateSubKey(name);

                        copyTo(srcSubKey, dstSubKey);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Registration.CopyTo() :\n" + ex.Message, "Exception in Registration");
            }
        }
        //Enable Registry will 
        public void EnableRegistry()
        {
            Microsoft.Win32.RegistryKey key3;
            Microsoft.Win32.RegistryKey key4;
            Microsoft.Win32.RegistryKey key5;
            Microsoft.Win32.RegistryKey key6;

            try
            {
                if (IsBackup)
                {
                    key3 = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("sip");
                    key3.SetValue("", "URL :BJNLync Protocol Handler");
                    key3.SetValue("URL Protocol", "");
                    key3.Close();

                    key4 = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("sip\\DefaultIcon");
                    key4.SetValue("", "C:\\Program Files (x86)\\BJN\\BJNLYNC\\OneClickVideo\\OneClickVideo.exe");
                    key4.Close();

                    key5 = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("sip\\shell\\open\\command");
                    key5.SetValue("", "\"C:\\Program Files (x86)\\BJN\\BJNLYNC\\OneClickVideo\\OneClickVideo.exe\" \"%1\"");
                    key5.Close();

                    //The below regestry changes will stop Lync client from overriding everytime it login
                    key6 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Office\\15.0\\Lync");
                    key6.SetValue("DisabledProtocolHandlerRegistrations", "4", Microsoft.Win32.RegistryValueKind.DWord);
                    key6.Close();
                }
                else
                {
                    MessageBox.Show("sip.backup failed in Registration::");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration.EnableRegistry() : \n" + ex.Message, "Exception in Registration");
            }
        }
    }
}
