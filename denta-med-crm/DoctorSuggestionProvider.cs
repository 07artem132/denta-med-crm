using AutoCompleteTextBox.Editors;
using denta_med_crm.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace denta_med_crm
{
    class DoctorSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable<string> ListOfDoctors => MainWindow.db.DoctorsHistory.Keys;

        public string GetExactSuggestion(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                (ListOfDoctors
                    .FirstOrDefault(doctor => string.Equals(doctor, filter, StringComparison.CurrentCultureIgnoreCase)));
        }

        public IEnumerable<string> GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;

            var result = ListOfDoctors
                   .Where(doctor => doctor.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) > -1)
                   .ToList();
            
            if (result.Count == 0)
                result.Add(filter);
            
            return result;
        }

        IEnumerable ISuggestionProvider.GetSuggestions(string filter)
        {
            return GetSuggestions(filter);
        }

        public void OnTextChanged(string prev, string value)
        {
            MainWindow.db?.OnDoctorRename(prev, value);
            System.Diagnostics.Debug.WriteLine(prev + " -> " + value);
        }

    }
}
