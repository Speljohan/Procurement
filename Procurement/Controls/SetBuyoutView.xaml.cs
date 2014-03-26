using System.Linq;
using System.Collections;
using System.Windows.Controls;
using System.Globalization;
using System.Text.RegularExpressions;
using System;

namespace Procurement.Controls
{
    public partial class SetBuyoutView : UserControl
    {
        public SetBuyoutView()
        {
            InitializeComponent();
            BuyoutValue.Text = "1";
            CurrentOfferValue.Text = "1";
            LastUpdated.Content = "";
        }

        public event BuyoutHandler SaveClicked;
        public event BuyoutHandler RemoveClicked;
        public delegate void BuyoutHandler(string buyoutAmount, string buyoutOrbType, string currentOfferAmount, string currentOfferType, bool highlight);

        public void SaveBuyout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BuyoutValue.Text == string.Empty)
                BuyoutValue.Text = "0";

            if (CurrentOfferValue.Text == string.Empty)
                CurrentOfferValue.Text = "0";
            
            SaveClicked(double.Parse(BuyoutValue.Text, CultureInfo.InvariantCulture).ToString(), ((ComboBoxItem)OrbType.SelectedItem).Content.ToString(),
                double.Parse(CurrentOfferValue.Text, CultureInfo.InvariantCulture).ToString(), ((ComboBoxItem)CurrentOfferOrbType.SelectedItem).Content.ToString(), Highlight.IsChecked == true);
            LastUpdated.Content = DateTime.Now;
        }

        private void Increase_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            updateValue(1, BuyoutValue);
        }

        private void Decrease_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            updateValue(-1, BuyoutValue);
        }

        private void CurrentOffer_Increase_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            updateValue(1, CurrentOfferValue);
        }

        private void CurrentOffer_Decrease_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            updateValue(-1, CurrentOfferValue);
        }

        public void SetValues(string buyoutAmount, string buyoutOrbType, string currentOfferAmount, string currentOfferOrbType, bool highlight, DateTime lastUpdated)
        {
            BuyoutValue.Text = buyoutAmount;
            OrbType.SelectedItem = OrbType.Items.Cast<ComboBoxItem>().First(i => i.Content.ToString() == buyoutOrbType);
            CurrentOfferValue.Text = currentOfferAmount;
            CurrentOfferOrbType.SelectedItem = OrbType.Items.Cast<ComboBoxItem>().First(i => i.Content.ToString() == currentOfferOrbType);
            Highlight.IsChecked = highlight;
            LastUpdated.Content = lastUpdated;
        }

        private void updateValue(int difference, TextBox txt)
        {
            var buyout = double.Parse(txt.Text, CultureInfo.InvariantCulture);
            buyout += difference;
            txt.Text = buyout.ToString(); 
        }

        private void BuyoutValue_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = validateInput(e.Text);
        }

        private static bool validateInput(string text)
        {
            return new Regex("[^0-9.]+").IsMatch(text);
        }

        private void RemoveBuyout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveClicked(null, null, null, null, false);
        }
    }
}
