using Core;

using System;
using System.Collections.Generic;
using System.Text;

namespace XperienceAdapter.Generator
{
    public interface IGenerator : IService
    {
        void GenerateContacts(string path);

        void GenerateActivities();

        void GeneratePersona();

        void GenerateContactGroup();

        void GenerateConversions();
    }
}
