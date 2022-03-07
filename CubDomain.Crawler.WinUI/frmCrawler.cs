using CubDomain.Crawler.Service;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CubDomain.Crawler.WinUI
{
    public partial class frmCrawler : Form
    {
        public frmCrawler()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
              {
                  try
                  {
                      btnStart.Invoke((MethodInvoker)delegate
                      {
                          btnStart.Enabled = false;
                          btnStart.Text = "Working ...";
                      });

                      DateTime currentDate = DateTime.Now;

                      CDomainService domainService = new CDomainService();
                      if (!string.IsNullOrEmpty(txtStartDate.Text))
                      {
                          currentDate = DateTime.Parse(txtStartDate.Text);
                      }
                      else
                      {
                          var last = domainService.GetSync(true, a => a.Domain != null, o => o.OrderBy(a => a.RegisterDate), string.Empty, 1).FirstOrDefault();
                          if (last != null)
                              currentDate = last.RegisterDate;
                      }

                      ChromeDriver chromeDriver = GetChromeDriver();
                      if (MessageBox.Show("okey?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                      {
                          while (currentDate.Subtract(DateTime.Parse("2018-05-07")).TotalDays > 0)
                          {
                              currentDate = currentDate.AddDays(-1);
                              string date = currentDate.ToString("yyyy-MM-dd");
                              int page = 1;
                              while (true)
                              {
                                  try
                                  {
                                      string url = $"https://www.cubdomain.com/domains-registered-by-date/{date}/{page}";
                                      chromeDriver.Navigate().GoToUrl(url);

                                      if (!chromeDriver.PageSource.Contains("Error 404"))
                                      {
                                          HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                                          htmlDocument.LoadHtml(chromeDriver.PageSource);

                                          if (chromeDriver.PageSource.Contains("Attention Required! | Cloudflare"))
                                          {
                                              if (MessageBox.Show("captcha?") == DialogResult.OK)
                                              {

                                              }
                                          }

                                          string dataRow = htmlDocument.DocumentNode.SelectNodes("/html/body/div[2]/div[1]/section/div[3]").FirstOrDefault().InnerText;

                                          string[] domainArray = dataRow.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(a => a != null && !string.IsNullOrEmpty(a.Trim())).ToArray();

                                          if (domainArray.Length == 0)
                                          {
                                              break;
                                          }

                                          string query = string.Empty;
                                          foreach (var domain in domainArray)
                                          {
                                              string domainString = domain.Trim().ToLower();
                                              try
                                              {
                                                  query += "INSERT INTO [dbo].[CDomain]([Domain],[RegisterDate],[Extention],[InsertUserAccountId],[InsertDateTime],[UpdateUserAccountId],[UpdateDateTime])" +
                                                  $"VALUES('{domainString}','{currentDate}','{Path.GetExtension(domainString)}','{Guid.Empty}','{DateTime.Now}','{Guid.Empty}','{DateTime.Now}')" +
                                                  $"\r\n";
                                                  lblDomainCount.Invoke((MethodInvoker)delegate
                                                  {
                                                      lblDomainCount.Text = (Convert.ToInt32(lblDomainCount.Text) + 1).ToString();
                                                  });
                                              }
                                              catch (Exception ex)
                                              {
                                                  log("SaveDomain", ex);
                                              }
                                          }
                                          var cDomainService = new CDomainService();
                                          int rows = cDomainService.SaveDomains(query);
                                          log(date + " - " + page + " - " + rows + " saved");
                                      }
                                      else break;
                                  }
                                  catch (Exception ex)
                                  {
                                      if (MessageBox.Show("error?" + ex.Message) == DialogResult.OK)
                                      {

                                      }
                                  }
                                  page++;
                              }
                          }
                      }
                      btnStart.Invoke((MethodInvoker)delegate
                      {
                          btnStart.Enabled = true;
                          btnStart.Text = "Start";
                      });
                  }
                  catch (Exception ex)
                  {
                      log("main", ex);
                      btnStart.Invoke((MethodInvoker)delegate
                      {
                          btnStart.Enabled = true;
                          btnStart.Text = "Start";
                      });
                  }
              });
        }
        private void log(string message, Exception ex = null)
        {
            lbLog.Invoke((MethodInvoker)delegate
            {
                if (ex != null)
                {
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }
                    message += " | " + ex.Message;
                }
                lbLog.Items.Insert(0, DateTime.Now.ToString() + "\t" + message);
            });
        }
        public static ChromeDriver GetChromeDriver()
        {
            try
            {
                int chromePort = 9409;
                //Process proc = new Process();
                //proc.StartInfo.FileName = ConfigurationManager.AppSettings["ChromePath"];

                //string userDir = "C:\\ChromeDriver\\";
                //if (!Directory.Exists(userDir))
                //{
                //    Directory.CreateDirectory(userDir);
                //}

                //userDir += "Chrome_Cubdomain";
                //proc.StartInfo.Arguments = "http://cubdomain.com/ --new-window --remote-debugging-port=" + chromePort;// + " --user-data-dir=" + userDir;
                //proc.Start();

                ChromeOptions chromeOptions = new ChromeOptions
                {
                    AcceptInsecureCertificates = true,
                    //DebuggerAddress = "127.0.0.1:" + chromePort
                };
                string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
                List<string> options = new List<string>() { "no-sandbox", "disable-gpu", "--user-agent=" + userAgent, "--lang=en" };

                chromeOptions.AddArguments(options);

                ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService();
                chromeService.HideCommandPromptWindow = true;

                return new ChromeDriver(chromeService, chromeOptions);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}