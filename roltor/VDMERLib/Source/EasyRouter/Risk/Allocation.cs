using System;
using System.Collections.Generic;
using System.Text;
using VDMERLib.EasyRouter.General;
using VDMERLib.EasyRouter.EasyRouterClient;
using System.Xml.Serialization;

namespace VDMERLib.EasyRouter.Risk
{
    public class Allocation : Dictionary<long, int>, IProfile
    {
        string m_strDescription;
        public Allocation(string strName)
        {
            m_strDescription = strName;
            ERCSClient.GetInstance().AddPersistedClass(this);
        }

        
        public void AddAccount(long AccountID, int PercentageAllocation)
        {
            this.Add(AccountID, PercentageAllocation);
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        #region IProfile Members

        public ScreenIDs ScreenID
        {
            get { return ScreenIDs.Allocation; }
        }

        public string InstanceID
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public string FormName
        {
            get
            {
                return "Allocations";
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool ReadProperties(System.Xml.XmlReader reader)
        {
            m_strDescription = reader.GetAttribute("Description");
            reader.Read();
            while (reader.Name == "Allocation") 
            {
                
                long AccountID = Int64.Parse(reader.GetAttribute("AccountID"));
                int AllocationPercentage = Int32.Parse(reader.GetAttribute("AllocationPercentage"));
                this.Add(AccountID, AllocationPercentage);
                reader.Read();

            } 
            return true;
        }

        public void WriteProperties(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Description", m_strDescription);
            foreach (KeyValuePair<long, int> kvp in this)
            {
                writer.WriteStartElement("Allocation");
                writer.WriteAttributeString("AccountID", kvp.Key.ToString());
                writer.WriteAttributeString("AllocationPercentage", kvp.Value.ToString());
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
