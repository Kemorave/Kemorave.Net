using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Kemorave.Wpf.Helper
{
    public static class TextBoxHelper
    {
        public static Regex VaildEmailRegex = new Regex(@"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$", RegexOptions.IgnoreCase);
        public static Regex VaildPhoneNumberRegex = new Regex(@"\(?\d{3}\)?[. -]? *\d{3}[. -]? *[. -]?\d{4}", RegexOptions.IgnoreCase);





        public static bool? GetEditMode(DependencyObject obj)
        {
            return (bool?)obj.GetValue(EditModeProperty);
        }

        public static void SetEditMode(DependencyObject obj, bool? value)
        {
            obj.SetValue(EditModeProperty, value);
        }

        // Using a DependencyProperty as the backing store for EditMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.RegisterAttached("EditMode", typeof(bool?), typeof(TextBoxHelper), new PropertyMetadata(null));

        

        public static DependencyProperty AllowEmailOnlyProperty = DependencyProperty.RegisterAttached("AllowEmailOnly", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, AllowEmailOnlyChanged));
        private static void AllowEmailOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox tx = d as TextBox;
                tx.LostFocus += (s1, a1) =>
                {
                    if (!VaildEmailRegex.IsMatch(tx.Text))
                    {
                        tx.SelectAll();
                    }
                };
            }
        }
        public static void SetAllowEmailOnlyProperty(DependencyObject dobj, bool value)
        {
            dobj.SetValue(AllowEmailOnlyProperty, value);
        }
        public static bool GetAllowEmailOnlyProperty(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(AllowEmailOnlyProperty);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public static DependencyProperty AllowMoneyOnlyProperty = DependencyProperty.RegisterAttached("AllowMoneyOnly", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, AllowMoneyOnlyChanged));
        private static void AllowMoneyOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox tx = d as TextBox;
                tx.Initialized += (s, a) => { tx.Text = "0"; };
                tx.LostFocus += (a, s) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tx.Text) && !string.IsNullOrEmpty(tx.Text))
                        {
                            if (double.TryParse(tx.Text.Replace("$", ""), out double value))
                            {
                                tx.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:c2}", value);
                                return;
                            }
                            tx.Text = "0";

                        }
                        else
                        {
                            tx.Text = "0";
                        }
                    }
                    catch (Exception)
                    {
                    }
                };
            }
        }


        public static void SetAllowMoneyOnlyProperty(DependencyObject dobj, bool value)
        {
            dobj.SetValue(AllowMoneyOnlyProperty, value);
        }
        public static bool GetAllowMoneyOnlyProperty(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(AllowMoneyOnlyProperty);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////
        public static DependencyProperty AllowNumbericOnlyProperty = DependencyProperty.RegisterAttached("AllowNumbericOnly", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, AllowNumbericOnlyChanged));
        private static void AllowNumbericOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox tx = d as TextBox;
                tx.TextChanged += (s, a) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tx.Text))
                        {

                            tx.Text = Convert.ToDecimal(tx.Text).ToString();
                        }
                        else
                        {
                            tx.Text = "0";
                        }
                    }
                    catch (Exception)
                    {
                        if (tx.Text.Length > 0)
                            tx.Text = tx.Text.Substring(0, tx.Text.Length - 1);
                        tx.Text = "0";
                    }
                };
            }

        }


        public static void SetAllowNumbericOnlyProperty(DependencyObject dobj, bool value)
        {
            dobj.SetValue(AllowNumbericOnlyProperty, value);
        }
        public static bool GetAllowNumbericOnlyProperty(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(AllowNumbericOnlyProperty);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////


        public static DependencyProperty AllowPhoneNumberOnlyProperty = DependencyProperty.RegisterAttached("AllowPhoneNumberOnly", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, AllowPhoneNumberOnlyChanged));
        private static void AllowPhoneNumberOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox tx = d as TextBox;
                tx.LostFocus += (s1, a1) =>
                {
                    if (!VaildPhoneNumberRegex.IsMatch(tx.Text))
                    {
                        tx.SelectAll();
                    }
                };
            }
        }


        public static void SetAllowPhoneNumberOnlyProperty(DependencyObject dobj, bool value)
        {
            dobj.SetValue(AllowPhoneNumberOnlyProperty, value);
        }
        public static bool GetAllowPhoneNumberOnlyProperty(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(AllowPhoneNumberOnlyProperty);
        }
    }
}
