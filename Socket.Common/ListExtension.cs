using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTSocket
{
    public class RecieverCollection
    {
        private List<Guid> ids = new List<Guid>();
        private List<BaseReciever> items = new List<BaseReciever>();
        public BaseReciever this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public BaseReciever this[Guid index]
        {
            get
            {
                return items?.Find(item => Guid.Equals(item.ID, index));
            }
            private set
            {
                items?.Find(item => Guid.Equals(item.ID, index))?.Copy(value);
            }
        }

        public void Add(BaseReciever newItems, bool overwrite = true)
        {
            if (newItems == null)
            {
                throw new Exception("Cannot add null element.");
            }
            else if (this[newItems.ID] != null)
            {
                if (overwrite)
                {
                    this[newItems.ID] = newItems;
                }
                else
                {
                    throw new Exception("Cannot add existed element.");
                }
            }
            else
            {
                ids.Add(newItems.ID);
                this.items.Add(newItems);
            }
        }

        public void Remove(BaseReciever newItems)
        {
            if (newItems == null)
            {
                //throw new Exception("Cannot remove null element.");
                return;
            }
            else if(this[newItems.ID] != null)
            {
                items.Remove(this[newItems.ID]);
            }
        }

        public bool Contains(Guid id)
        {
            return this[id] != null;
        }
    }
}
