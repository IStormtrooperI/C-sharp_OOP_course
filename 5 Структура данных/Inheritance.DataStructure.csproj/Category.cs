using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        private readonly string categoryName;
        private readonly MessageType messageType;
        private readonly MessageTopic messageTopic;

        internal Category(string categoryName, MessageType messageType, MessageTopic messageTopic)
        {
            this.categoryName = categoryName;
            this.messageType = messageType;
            this.messageTopic = messageTopic;
        }

        public bool Equals(Category otherCategory)
        {
            try
            {
                var isEqual = this.categoryName == otherCategory.categoryName &&
                    this.messageType == otherCategory.messageType &&
                    this.messageTopic == otherCategory.messageTopic;
                return isEqual;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = this.categoryName.GetHashCode() + 2 * (int)this.messageType + (int)this.messageTopic;
            return hashCode;
        }

        public override string ToString()
        {
            return $"{this.categoryName}.{this.messageType:G}.{this.messageTopic:G}";
        }

        public int CompareTo(object compareObj)
        {
            if (compareObj == null)
            {
                return 1;
            }
            var otherCategory = (Category)compareObj;
            var compareName = string.Compare(this.categoryName, otherCategory.categoryName);
            if (compareName != 0)
            {
                return compareName;
            }
            var compareType = (int)this.messageType - (int)otherCategory.messageType;
            if(compareType != 0)
            {
                return compareType;
            }
            return (int)this.messageTopic - (int)otherCategory.messageTopic;
        }

        public static bool operator <=(Category firstCategory, Category secondCategory)
        {
            return !(firstCategory > secondCategory);
        }

        public static bool operator >=(Category firstCategory, Category secondCategory)
        {
            return !(firstCategory < secondCategory);
        }

        public static bool operator <(Category firstCategory, Category secondCategory)
        {
            return firstCategory.CompareTo(secondCategory) < 0;
        }

        public static bool operator >(Category firstCategory, Category secondCategory)
        {
            return firstCategory.CompareTo(secondCategory) > 0;
        }
    }
}
