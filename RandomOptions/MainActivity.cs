using System.Xml;

namespace RandomOptions
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public static class DataFilePath
        {
            public static readonly string template = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "template.xml");
        }
        public static readonly string version = "1.1.1.20241013";
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            /*Button templateButton = FindViewById<Button>(Resource.Id.templateButton)!;
            templateButton.Click += (object? sender, EventArgs e)=>
            {

            };
            templateButton.LongClick += (object? sender, Android.Views.View.LongClickEventArgs e)=>
            { 

            };*/
            //到时候制作可以在多个模板之间切换的功能

            EditText templateEditBox= FindViewById<EditText>(Resource.Id.templateEditBox)!;
            templateEditBox.FocusChange+= (object? sender, Android.Views.View.FocusChangeEventArgs e)=>
            {
                //保存模板数据
                if (e.HasFocus == false)
                {
                    SaveTextData(templateEditBox.Text!);
                }
            };
            TextView randomText = FindViewById<TextView>(Resource.Id.randomText)!;
            Button startButton = FindViewById<Button>(Resource.Id.startButton)!;
            bool stopSignal;
            startButton.Click += (object? sender, EventArgs e) =>
            {               
                if (startButton.Text == "开始")
                {
                    templateEditBox.Enabled = false;
                    stopSignal = false;
                    Thread thread = new(() =>
                    {
                        string[] randomTemplate = templateEditBox.Text!.Split("\n");
                        int cs = -1;
                        while (!stopSignal)
                        {
                            if (cs < randomTemplate.Length-1) cs++;
                            else cs = 0;
                            RunOnUiThread(() =>
                            {
                                randomText.Text= randomTemplate[cs];
                            });
                            Thread.Sleep(10);
                        }
                    });
                    thread.Start();
                    startButton.Text = "停止";
                }
                else
                {
                    templateEditBox.Enabled = true;
                    stopSignal =true;
                    startButton.Text = "开始";
                }
            };
            TextView guanyu= FindViewById<TextView>(Resource.Id.guanyu)!;
            guanyu.Click += (object? sender, EventArgs e) =>
            {
                var msgbox = new Android.App.AlertDialog.Builder(this);
                msgbox.SetTitle("关于");
                msgbox.SetMessage("程序名：随机选项\r\n版本：V" + version + "\r\n版权所有者：Copyright (C) 2024 Hgnim, All rights reserved." +
                    "\r\nGithub: https://github.com/Hgnim/RandomOptions");//每完成一个项目，就往此处加版本号
                msgbox.SetPositiveButton("确定", delegate
                {
                });
                msgbox.Show();
            };



            //数据文件处理块
            if (!File.Exists(DataFilePath.template))
            {
                File.Create(DataFilePath.template).Close();
                XmlTextWriter xmlWriter = new(DataFilePath.template, System.Text.Encoding.GetEncoding("utf-8")) { Formatting = System.Xml.Formatting.Indented };

                xmlWriter.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                xmlWriter.WriteStartElement("template");

                xmlWriter.WriteStartElement("item");
                xmlWriter.WriteAttributeString("id", "0");
                xmlWriter.WriteAttributeString("text", "");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteFullEndElement();
                xmlWriter.Close();
            }
            else
            {
                XmlDocument xmlDoc = new();
                XmlNode xmlRoot;
                xmlDoc.Load(DataFilePath.template);
                xmlRoot = xmlDoc.SelectSingleNode("template")!; //进入对应节点
                XmlNodeList xmlNL = xmlRoot.ChildNodes;
                foreach (XmlNode xn in xmlNL) //循环扫描节点
                {
                    XmlElement xmlE = (XmlElement)xn;
                    switch (xmlE.GetAttribute("id"))
                    {
                        case "0":
                            templateEditBox.Text = xmlE.GetAttribute("text");
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 保存输入框内的文本数据
        /// </summary>
        /// <param name="data">文本数据</param>
        void SaveTextData(string data)
        {
            //保存模板数据
            XmlDocument xmlDoc = new();
            XmlNodeList xmlNL;
            XmlElement xmlEle;
            xmlDoc.Load(DataFilePath.template);
            xmlNL = xmlDoc.SelectSingleNode("template")!.ChildNodes;
            foreach (XmlNode xn in xmlNL) //循环扫描节点
            {
                xmlEle = (XmlElement)xn;
                if (xmlEle.GetAttribute("id") == "0")
                {
                    xmlEle.SetAttribute("text", data);
                    break;
                }
            }
            xmlDoc.Save(DataFilePath.template);
        }
    }
}