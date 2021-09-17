using AutoEquThreeMeterModelScan.Configs;
using HkMvCodeReaderManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoEquThreeMeterModelScan.view.SetView
{
    /// <summary>
    /// ComConfig.xaml 的交互逻辑
    /// </summary>
    public partial class ComConfig : Window
    {

        public CodeReaderManager codeReaderManager;

        public CoderReader coderReader_inside { get; private set; }

        public CoderReader coderReader_outside { get; private set; }

        public CoderReaderConfigInfo coderReaderConfigInfo;
        public ComConfig()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                coderReaderConfigInfo = new CoderReaderConfigInfo();
                codeReaderManager = new CodeReaderManager();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
            codeReaderList01.ItemsSource = codeReaderManager.coderReaders;

            coderReader_inside = codeReaderManager.GetCoderReaderByIp(coderReaderConfigInfo.InsideCodeReaderIp);
            if (coderReader_inside != null)
            {

                codeReaderList01.SelectedItem = coderReader_inside;

                if (coderReaderConfigInfo.InsideIsOpen)
                {
                    coderReader_inside.ConnectReader();

                }
            }
            textBox_inside_outtime.Text = coderReaderConfigInfo.OutsideCodeReaderOutTime.ToString();
            textBox_outside_outtime.Text = coderReaderConfigInfo.InsideCodeReaderOutTime.ToString();

            codeReaderList01.DisplayMemberPath = "Ip";
            codeReaderList02.ItemsSource = codeReaderManager.coderReaders;

            coderReader_outside = codeReaderManager.GetCoderReaderByIp(coderReaderConfigInfo.OutsideCoderReaderIp);
            if (coderReader_outside != null)
            {
                codeReaderList02.SelectedItem = coderReader_outside;
                if (coderReaderConfigInfo.OutsideIsOpen)
                {
                    coderReader_outside.ConnectReader();
                }

            }

            codeReaderList02.DisplayMemberPath = "Ip";

            insideIsopenFlage.SetBinding(TextBlock.TextProperty, new Binding("IsOpen") { Source = coderReader_inside });
            outsideIsopenFlage.SetBinding(TextBlock.TextProperty, new Binding("IsOpen") { Source = coderReader_outside });

        }

        /// <summary>
        /// 测试内侧读码器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string IP = ((CoderReader)codeReaderList01.SelectedItem)?.Ip;
            if (!string.IsNullOrEmpty(IP))
            {

                coderReader_inside = codeReaderManager.GetCoderReaderByIp(IP);
                textBox_inside.Text = "";
                try
                {
                    string str = coderReader_inside.ReadAnCode(1000);

                    textBox_inside.Text = str;
                    Console.WriteLine("内侧读码器读取结果：" + str);

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }


            }

        }

        /// <summary>
        ///测试外侧读码器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string IP = ((CoderReader)codeReaderList02.SelectedItem)?.Ip;
            if (!string.IsNullOrEmpty(IP))
            {

                coderReader_outside = codeReaderManager.GetCoderReaderByIp(IP);

                textBox_outside.Text = "";
                try
                {
                    string str = coderReader_outside.ReadAnCode(1000);
                    textBox_outside.Text = str;
                    Console.WriteLine("外侧读码器读取结果：" + str);

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }

            }
        }

        /// <summary>
        /// 保存读码器配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            if (coderReader_inside == coderReader_outside)
            {
                MessageBox.Show("内外侧读码器不能是同一个");

                return;

            }

        }

        /// <summary>
        /// 关闭页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            textBox_outside.Text = "";
            textBox_outside.Text = "";
        }

        /// <summary>
        /// 连接内侧读码器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            coderReader_inside.ConnectReader();

        }
        /// <summary>
        /// 断开内侧读码器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            coderReader_inside.CloseCodeReader();
        }

        /// <summary>
        /// 连接外侧读码器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            coderReader_outside.ConnectReader();
        }

        /// <summary>
        /// 断开外侧读码器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            coderReader_outside.CloseCodeReader();
        }

        /// <summary>
        /// 全部连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            coderReader_outside.ConnectReader();
            coderReader_inside.ConnectReader();
        }

        /// <summary>
        /// 断开全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            coderReader_outside.CloseCodeReader();
            coderReader_inside.CloseCodeReader();
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            string insideTimeout = textBox_inside_outtime.Text;

            string outsideTimeout = textBox_outside_outtime.Text;

            try
            {
                int insidetime = int.Parse(insideTimeout);

                int outtime = int.Parse(outsideTimeout);

                if (insidetime < 100 || outtime < 100)
                {

                    MessageBox.Show("超时时间不能低于100ms");
                    return;

                }

                if (insidetime > 10000 || outtime < 10000)
                {

                    MessageBox.Show("超时时间不能大于10000ms");
                    return;
                }



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void codeReaderList01_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            coderReader_inside = (CoderReader)codeReaderList01.SelectedItem;
            insideIsopenFlage.SetBinding(TextBlock.TextProperty, new Binding("IsOpen") { Source = coderReader_inside });
        }

        private void codeReaderList02_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            coderReader_outside = (CoderReader)codeReaderList02.SelectedItem;

            outsideIsopenFlage.SetBinding(TextBlock.TextProperty, new Binding("IsOpen") { Source = coderReader_outside });
        }
    }
}
