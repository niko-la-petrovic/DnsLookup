using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DnsLookup.ViewModels
{
    public class DnsViewModel : INotifyPropertyChanged
    {
        private string selectedDomainName;
        private IEnumerable<string> domainNames;
        private float opacity = 1.0f;

        public float Opacity
        {
            get => opacity; set
            {
                if (opacity == value) return;

                opacity = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> DomainNames
        {
            get => domainNames; set
            {
                domainNames = value.ToList();
                OnPropertyChanged();
            }
        }

        public string SelectedDomainName
        {
            get => selectedDomainName; set
            {
                if (selectedDomainName == value) return;

                selectedDomainName = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            OnPropertyChanged(property.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
