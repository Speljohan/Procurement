﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using POEApi.Model;
using Procurement.ViewModel;
using POEApi.Infrastructure;

namespace Procurement.Controls
{
    public partial class ItemDisplay : UserControl
    {
        private static List<Popup> annoyed = new List<Popup>();
        private static ResourceDictionary expressionDarkGrid;

        private TextBlock textblock;

        public ItemDisplay()
        {
            InitializeComponent();
            expressionDarkGrid = expressionDarkGrid ?? Application.LoadComponent(new Uri("/Procurement;component/Controls/ExpressionDarkGrid.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;

            this.Loaded += new RoutedEventHandler(ItemDisplay_Loaded);
        }

        public static void ClosePopups()
        {
            closeOthersButNot(new Popup());
        }

        void ItemDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            ItemDisplayViewModel vm = this.DataContext as ItemDisplayViewModel;
            Image i = vm.getImage();

            UIElement socket = vm.getSocket();

            this.MainGrid.Children.Add(i);

            if (socket != null)
                doSocketOnHover(socket, i);

            i.ContextMenu = getContextMenu();

            this.Height = i.Height;
            this.Width = i.Width;
            this.Loaded -= new RoutedEventHandler(ItemDisplay_Loaded);

            resyncText();
        }

        private void resyncText()
        {
            ItemDisplayViewModel vm = this.DataContext as ItemDisplayViewModel;
            Item item = vm.Item;

            if ((item is Currency))
                return;

            MenuItem setBuyout = new MenuItem();
            string buyoutValue = string.Empty;

            if (Settings.Buyouts.ContainsKey(item.UniqueIDHash))
            {
                buyoutValue = "bo: " + Settings.Buyouts[item.UniqueIDHash].Buyout;
                if (Settings.Buyouts[item.UniqueIDHash].CurrentOffer != string.Empty)
                {
                    buyoutValue += "\nco: " + Settings.Buyouts[item.UniqueIDHash].CurrentOffer;
                }

            }


            if (textblock != null)
                this.MainGrid.Children.Remove(textblock);

            textblock = new TextBlock();
            textblock.Text = buyoutValue;
            textblock.IsHitTestVisible = false;
            textblock.Margin = new Thickness(1, 1, 0, 0);
            this.MainGrid.Children.Add(textblock);
        }

        private void doSocketAlwaysOver(UIElement socket)
        {
            this.MainGrid.Children.Add(socket);
        }

        private void doSocketOnHover(UIElement socket, Image i)
        {
            NonTopMostPopup popup = new NonTopMostPopup();
            popup.PopupAnimation = PopupAnimation.Fade;
            popup.StaysOpen = true;
            popup.Child = socket;
            popup.Placement = PlacementMode.Center;
            popup.PlacementTarget = i;
            popup.AllowsTransparency = true;
            i.MouseEnter += (o, ev) =>
            {
                closeOthersButNot(popup);
                popup.IsOpen = true;
            };

            i.MouseLeave += (o, ev) =>
            {
                Rect rect = System.Windows.Media.VisualTreeHelper.GetDescendantBounds(i);
                if (!rect.Contains(ev.GetPosition(o as IInputElement)))
                    popup.IsOpen = false;
            };

            this.MainGrid.Children.Add(popup);
            annoyed.Add(popup);
        }

        private ContextMenu getContextMenu()
        {
            ItemDisplayViewModel vm = this.DataContext as ItemDisplayViewModel;
            Item item = vm.Item;

            ContextMenu menu = new ContextMenu();
            menu.Background = Brushes.Black;         
            
            menu.Resources = expressionDarkGrid;

            if (!(item is Currency))
            {
                MenuItem setBuyout = new MenuItem();

                var buyoutControl = new SetBuyoutView();

                if (Settings.Buyouts.ContainsKey(item.UniqueIDHash))
                {
                    var itemInfo = Settings.Buyouts[item.UniqueIDHash];
                    var boPrice = itemInfo.Buyout.Split(' ');
                    var coPrice = itemInfo.CurrentOffer.Split(' ');
                    buyoutControl.SetValues(boPrice[0], CurrencyAbbreviationMap.Instance.FromAbbreviation(boPrice[1]),
                        coPrice[0], CurrencyAbbreviationMap.Instance.FromAbbreviation(coPrice[1]), itemInfo.Highlight, itemInfo.LastUpdated);
                }

                setBuyout.Header = buyoutControl;
                buyoutControl.SaveClicked += new SetBuyoutView.BuyoutHandler(buyoutView_SaveClicked);
                buyoutControl.RemoveClicked += new SetBuyoutView.BuyoutHandler(buyoutControl_RemoveClicked);
                menu.Items.Add(setBuyout);
            }

            return menu;
        }

        void buyoutControl_RemoveClicked(string buyoutAmount, string buyoutOrbType, string currentOfferAmount, string currentOfferType, bool highlight)
        {
            ItemDisplayViewModel vm = this.DataContext as ItemDisplayViewModel;
            Item item = vm.Item;

            Settings.Buyouts.Remove(item.UniqueIDHash);
            Settings.Save();

            resyncText();
        }

        void buyoutView_SaveClicked(string buyoutAmount, string buyoutOrbType, string currentOfferAmount, string currentOfferType, bool highlight)
        {
            var boAbbreviation = CurrencyAbbreviationMap.Instance.FromCurrency(buyoutOrbType);
            var coAbbreviation = CurrencyAbbreviationMap.Instance.FromCurrency(currentOfferType);
           
            ItemDisplayViewModel vm = this.DataContext as ItemDisplayViewModel;
            Item item = vm.Item;
            ShopEntryInfo info = new ShopEntryInfo(item.UniqueIDHash, string.Format("{0} {1}", buyoutAmount, boAbbreviation), string.Format("{0} {1}", currentOfferAmount, coAbbreviation), highlight, DateTime.Now);

            if (Settings.Buyouts.ContainsKey(item.UniqueIDHash))
                Settings.Buyouts.Remove(item.UniqueIDHash);

            Settings.Buyouts.Add(item.UniqueIDHash, info);

            Settings.Save();

            resyncText();
        }

        public static void closeOthersButNot(Popup current)
        {
            List<Popup> others = annoyed.Where(p => p.IsOpen && !object.ReferenceEquals(current, p)).ToList();
            Task.Factory.StartNew(() => others.ToList().ForEach(p => p.Dispatcher.Invoke((Action)(() => { p.IsOpen = false; }))));
        }
    }
}
