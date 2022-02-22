using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VisualLayer.Controles
{
    public enum SearchTrigger
    {
        OnButtonClick, OnTextChanged
    }

    public enum SearchMode
    {
        Strict, Contains, StartsWith, EndsWith
    }

    public class SeachResult
    {
        public IEnumerable<object> Result { get; private set; }

        public SeachResult(IEnumerable<object> result)
        {
            Result = result;
        }
    }
    /// <summary>
    /// Lógica de interacción para SearchBar.xaml
    /// </summary>
    public partial class SearchBar : UserControl
    {
        #region Propiedades
        public string[] PropietiesToSearch => SearchByProperties.Split(',', ' ');



        public IEnumerable<object> SearchResult
        {
            get { return (IEnumerable<object>)GetValue(SearchResultProperty); }
            set { SetValue(SearchResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchResult.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchResultProperty =
            DependencyProperty.Register("SearchResult", typeof(IEnumerable<object>), typeof(SearchBar), new PropertyMetadata(default(IEnumerable<object>)));



        #region HintText
        public string HintText
        {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for HintText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register("HintText", typeof(string), typeof(SearchBar), new PropertyMetadata(string.Empty));
        #endregion HintText


        #region HasClearButton
        public bool HasClearButton
        {
            get => (bool)GetValue(HasClearButtonProperty);
            set => SetValue(HasClearButtonProperty, value);
        }

        // Using a DependencyProperty as the backing store for HasClearButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasClearButtonProperty =
            DependencyProperty.Register("HasClearButton", typeof(bool), typeof(SearchBar), new PropertyMetadata(true));
        #endregion HasClearButton


        #region SearchSource
        public IEnumerable<object> SearchSource
        {
            get => (IEnumerable<object>)GetValue(SearchSourceProperty);
            set => SetValue(SearchSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for SearchSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchSourceProperty =
            DependencyProperty.Register("SearchSource", typeof(IEnumerable<object>), typeof(SearchBar), new PropertyMetadata(null, SearchSorceChanged));

        private static void SearchSorceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is SearchBar sb)
            {
                sb.SearchResult = sb.SearchSource;
            }
        }

        #endregion SearchSource


        #region SearchTrigger
        public SearchTrigger SearchTrigger
        {
            get => (SearchTrigger)GetValue(SearchTriggerProperty);
            set => SetValue(SearchTriggerProperty, value);
        }

        // Using a DependencyProperty as the backing store for SearchMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchTriggerProperty =
            DependencyProperty.Register("SearchTrigger", typeof(SearchTrigger), typeof(SearchBar), new PropertyMetadata(SearchTrigger.OnButtonClick, SearchTriggerPropertyChanged));

        private static void SearchTriggerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((SearchTrigger)e.NewValue == SearchTrigger.OnTextChanged)
            {
                (d as SearchBar).SearchBox.TextChanged += (d as SearchBar).SearchBox_TextChanged;
            }
            else
            {
                (d as SearchBar).SearchBox.TextChanged -= (d as SearchBar).SearchBox_TextChanged;
            }
        }

        #endregion SearchTrigger


        #region SearchMode


        public SearchMode SearchMode
        {
            get => (SearchMode)GetValue(SearchModeProperty);
            set => SetValue(SearchModeProperty, value);
        }

        // Using a DependencyProperty as the backing store for SearchMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchModeProperty =
            DependencyProperty.Register("SearchMode", typeof(SearchMode), typeof(SearchBar), new PropertyMetadata(SearchMode.Strict));


        #endregion SearchMode


        #region SearchByProperties


        public string SearchByProperties
        {
            get => (string)GetValue(SearchByPropertiesProperty);
            set => SetValue(SearchByPropertiesProperty, value);
        }

        // Using a DependencyProperty as the backing store for SearchByProperties.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchByPropertiesProperty =
            DependencyProperty.Register("SearchByProperties", typeof(string), typeof(SearchBar), new PropertyMetadata(string.Empty));


        #endregion SearchByProperties


        #region SearchValue


        public string SearchValue
        {
            get => (string)GetValue(SearchValueProperty);
            set => SetValue(SearchValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for SearchValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchValueProperty =
            DependencyProperty.Register("SearchValue", typeof(string), typeof(SearchBar), new PropertyMetadata(string.Empty));


        #endregion SearchValue


        #region IsCaseSesitive


        public bool IsCaseSensitive
        {
            get { return (bool)GetValue(IsCaseSensitiveProperty); }
            set { SetValue(IsCaseSensitiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCaseSensitive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCaseSensitiveProperty =
            DependencyProperty.Register("IsCaseSensitive", typeof(bool), typeof(SearchBar), new PropertyMetadata(false));


        #endregion IsCaseSensitive
        #endregion Propiedades

        #region Eventos
        public delegate void SearchDoneDelegate(SeachResult e);
        public event SearchDoneDelegate SearchDone;
        #endregion Eventos

        #region Metodos
        public void Clear()
        {
            SearchBox.Clear();
        }
        #endregion Metodos

        public SearchBar()
        {
            InitializeComponent();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchResult = Search(SearchSource, SearchValue, SearchMode, PropietiesToSearch);
            SearchDone?.Invoke(new SeachResult(SearchResult));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<object> result = Search(SearchSource, SearchValue, SearchMode, PropietiesToSearch);
            SearchDone?.Invoke(new SeachResult(result));
        }

        private IEnumerable<object> Search(IEnumerable<object> source, string value, SearchMode searchMode, params string[] propertyNames)
        {
            List<object> result = new List<object>();

            if (value == null) return source;

            foreach (object obj in source)
            {
                bool isAdded = false;
                foreach (string s in propertyNames)
                {
                    if (isAdded) break;
                    System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(s);

                    if (prop == null) break;
                    string propValue = prop.GetValue(obj).ToString();

                    if (!IsCaseSensitive)
                    {
                        propValue = propValue.ToLower();
                        value = value.ToLower();
                    }

                    switch (searchMode)
                    {
                        case SearchMode.Strict:
                            if (propValue.Equals(value))
                            {
                                result.Add(obj);
                                isAdded = true;
                                break;
                            }
                            break;

                        case SearchMode.Contains:
                            if (propValue.Contains(value))
                            {
                                result.Add(obj);
                                isAdded = true;
                                break;
                            }
                            break;

                        case SearchMode.StartsWith:
                            if (propValue.StartsWith(value))
                            {
                                result.Add(obj);
                                isAdded = true;
                                break;
                            }
                            break;

                        case SearchMode.EndsWith:
                            if (propValue.EndsWith(value))
                            {
                                result.Add(obj);
                                isAdded = true;
                                break;
                            }
                            break;
                    }

                }
            }
            return result;
        }
    }
}
