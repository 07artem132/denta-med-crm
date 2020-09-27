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

        public IEnumerable<Doctor> ListOfDoctors { get; set; }

        public Doctor GetExactSuggestion(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                ListOfDoctors
                    .FirstOrDefault(doctor => string.Equals(doctor.Name, filter, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerable<Doctor> GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;

            var result = ListOfDoctors
                   .Where(doctor => doctor.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) > -1)
                   .ToList();
            
            if (result.Count == 0)
                result.Add(new Doctor(filter));
            
            return result;
        }

        IEnumerable ISuggestionProvider.GetSuggestions(string filter)
        {
            return GetSuggestions(filter);
        }

        public DoctorSuggestionProvider()
        {
            ListOfDoctors = MainWindow.db.DoctorsHistory;
        }
    }
}
