﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    public partial class Control
    {
        public partial class ControlCollection : ArrangedElementCollection, IList, ICloneable
        {
            Gtk.Container __ownerControl;
            Control __owner;
            Type __itemType;
            public ControlCollection(Control owner)
            {
                __ownerControl = owner.GtkControl as Gtk.Container;
                __owner = owner;
                __ownerControl.Mapped += __ownerControl_Mapped;
            }

            public ControlCollection(Control owner, Gtk.Container ownerContainer)
            {
                __ownerControl = ownerContainer;
                __owner = owner;
                __ownerControl.Mapped += __ownerControl_Mapped;
            }

            private void __ownerControl_Mapped(object sender, EventArgs e)
            {
                Gtk.Widget parent = (Gtk.Widget)sender;

                if (__ownerControl is Gtk.Overlay lay)
                {
                    foreach (object item in this)
                    {
                        if (item is Control control)
                        {
                            control.Widget.MarginStart = Math.Max(0, control.Widget.MarginStart + Offset.X);
                            control.Widget.MarginTop = Math.Max(0, control.Widget.MarginTop + Offset.Y);
                            SetMarginEnd(lay, control.Widget);
                        }
                        else if (item is Gtk.Widget widget)
                        {
                            widget.MarginStart = Math.Max(0, widget.MarginStart + Offset.X);
                            widget.MarginTop = Math.Max(0, widget.MarginTop + Offset.Y);
                            SetMarginEnd(lay, widget);
                        }
                    }
                }
            }
            internal Drawing.Point Offset = new Drawing.Point(0, 0);

            private void NativeAdd(object item)
            {
                try
                {
                    if (item is Control icontrol)
                    {
                        icontrol.Parent = __owner;
                    }
                    if (__ownerControl is Gtk.Overlay lay)
                    {
                        if (item is StatusStrip statusbar)
                        {
                            if (__owner is Form form)
                            {
                                statusbar.self.Halign = Gtk.Align.Fill;
                                statusbar.self.Valign = Gtk.Align.Start;
                                statusbar.self.Expand = false;
                                statusbar.self.MarginStart = 0;
                                statusbar.self.MarginTop = 0;
                                statusbar.self.MarginEnd = 0;
                                statusbar.self.MarginBottom = 0;
                                Gtk.Overlay overlay = new Gtk.Overlay();
                                overlay.HeightRequest = statusbar.Height;
                                overlay.AddOverlay(statusbar.self);
                                form.self.ContentArea.PackEnd(overlay, false, false, 0);
                            }
                        }
                        else if (item is Control control)
                        {
                            lay.AddOverlay(control.Widget);
                            lay.WidthRequest = Math.Max(0, Math.Max(lay.AllocatedWidth, control.Widget.MarginStart + control.Widget.WidthRequest + control.Widget.MarginEnd));
                            lay.HeightRequest = Math.Max(0, Math.Max(lay.AllocatedHeight, control.Widget.MarginTop + control.Widget.HeightRequest + control.Widget.MarginBottom));
                            SetMarginEnd(lay, control.Widget);
                            control.DockChanged += Control_DockChanged;
                            control.AnchorChanged += Control_AnchorChanged;
                        }
                        else if (item is Gtk.Widget widget)
                        {
                            lay.AddOverlay(widget);
                            lay.WidthRequest = Math.Max(lay.WidthRequest, widget.MarginStart + widget.WidthRequest + widget.MarginEnd);
                            lay.HeightRequest = Math.Max(lay.HeightRequest, widget.MarginTop + widget.HeightRequest + widget.MarginBottom);
                            SetMarginEnd(lay, widget);
                        }
                    }
                    else if (__ownerControl is Gtk.Fixed lay2)
                    {
                        if (item is Control con)
                        {
                            lay2.Put(con.Widget, Offset.X, Offset.Y);
                        }
                        else if (item is Gtk.Widget widget)
                        {
                            lay2.Put(widget, Offset.X, Offset.Y);
                        }
                    }
                    else if (__ownerControl is Gtk.Layout lay3)
                    {
                        if (item is Control con)
                        {
                            lay3.Put(con.Widget, Offset.X, Offset.Y);
                        }
                        else if (item is Gtk.Widget widget)
                        {
                            lay3.Put(widget, Offset.X, Offset.Y);
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (__ownerControl.IsRealized)
                    {
                        if (item is Control con)
                            con.Widget.ShowAll();
                        else if (item is Gtk.Widget widget)
                            widget.ShowAll();
                    }
                }
            }
            private void NativeAdd()
            {
                try
                {
                    foreach (object item in this)
                    {
                        NativeAdd(item);
                    }
                }
                catch
                {
                    throw;
                }
            }

            private void Control_AnchorChanged(object sender, EventArgs e)
            {
                Control control = sender as Control;
                if (control.Widget.Parent is Gtk.Overlay lay)
                {
                    SetMarginEnd(lay, control.Widget);
                }
            }
            private void Control_DockChanged(object sender, EventArgs e)
            {
                Control control = sender as Control;
                if (control.Widget.Parent is Gtk.Overlay lay)
                {
                    SetMarginEnd(lay, control.Widget);
                }
            }
            private void SetMarginEnd(Gtk.Overlay lay, Gtk.Widget widget)
            {
                if (widget.Halign == Gtk.Align.End || widget.Halign == Gtk.Align.Fill)
                {
                    if (widget.WidthRequest > 0)
                        widget.MarginEnd = Math.Max(0, lay.AllocatedWidth - widget.MarginStart - widget.WidthRequest);
                }
                if (widget.Valign == Gtk.Align.End || widget.Valign == Gtk.Align.Fill)
                {
                    if (widget.HeightRequest > 0)
                        widget.MarginBottom = Math.Max(0, lay.AllocatedHeight - widget.MarginTop - widget.HeightRequest);
                }
            }
            public virtual void Add(Gtk.Widget value)
            {
                NativeAdd(value);
            }
            public void AddWidget(Gtk.Widget item, Control control)
            {
                control.Parent = __owner;
                InnerList.Add(new ArrangedElementWidget(item));
            }
            public virtual void Add(Type itemType, Control item)
            {
                //重载处理
                this.Add(item);
            }


            public object Clone()
            {
                ControlCollection ccOther = new ControlCollection(__owner, __ownerControl);
                ccOther.InnerList.AddRange(InnerList);
                return ccOther;
            }

            public virtual bool ContainsKey(string? key)
            {
                return IsValidIndex(IndexOfKey(key));
            }

            public virtual void Add(Control? value)
            {
                if (value is null)
                {
                    return;
                }
                NativeAdd(value);
                InnerList.Add(value);
            }

            int IList.Add(object? control)
            {
                if (control is Control c)
                {
                    Add(c);
                    return IndexOf(c);
                }
                else
                {
                    throw new ArgumentException("ControlBadControl {0}", nameof(control));
                }
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public virtual void AddRange(params Control[] controls)
            {
                ArgumentNullException.ThrowIfNull(controls);

                if (controls.Length == 0)
                {
                    return;
                }
                foreach (Control item in controls)
                {
                    Add(item);
                }
            }

            object ICloneable.Clone()
            {
                ControlCollection ccOther = new ControlCollection(__owner, __ownerControl);
                ccOther.InnerList.AddRange(InnerList);
                return ccOther;
            }

            public bool Contains(Control? control) => ((IList)InnerList).Contains(control);

            public Control[] Find(string key, bool searchAllChildren)
            {
                List<IArrangedElement> foundControls = InnerList.FindAll(o =>
                {
                    if (o is Control con)
                    {
                        return con.Name == key;
                    }
                    else if (o is ArrangedElementWidget widget)
                    {
                        return widget.GetWidget?.Name == key;
                    }
                    else { return false; }
                });
                if (foundControls == null)
                    return new Control[0];
                else
                    return foundControls.ConvertAll(o => o as Control).ToArray();
            }

            public override IEnumerator GetEnumerator()
            {
                return InnerList.GetEnumerator();
            }

            public int IndexOf(Control? control) => ((IList)InnerList).IndexOf(control);

            public virtual int IndexOfKey(string? key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return -1;
                }
                return InnerList.FindIndex(o =>
                {
                    if (o is Control con)
                    {
                        return con.Name == key;
                    }
                    else if (o is ArrangedElementWidget widget)
                    {
                        return widget.GetWidget?.Name == key;
                    }
                    else { return false; }
                });
            }

            private bool IsValidIndex(int index)
            {
                return ((index >= 0) && (index < Count));
            }

            public Control Owner { get => __owner; }

            public virtual void Remove(Control? value)
            {
                if (value is null)
                {
                    return;
                }
                InnerList.Remove(value);
                __ownerControl.Remove(value.Widget);
            }

            void IList.Remove(object? element)
            {
                if (element is Control control)
                    Remove(control);
                else if (element is Gtk.Widget widget)
                {
                    __ownerControl.Remove(widget);
                    int index = this.IndexOfKey(widget.Name);
                    if (index >= 0)
                        InnerList.RemoveAt(index);
                }
            }

            public void RemoveAt(int index)
            {
                IArrangedElement element = InnerList[index];
                if (element is Control control)
                    Remove(control);
                else if (element is ArrangedElementWidget widget)
                {
                    InnerList.RemoveAt(index);
                    __ownerControl.Remove(widget.GetWidget);
                }
            }

            public virtual void RemoveByKey(string? key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            public new virtual Control this[int index]
            {
                get
                {
                    Control control = (Control)InnerList[index]!;
                    return control;
                }
            }

            public virtual Control? this[string? key]
            {
                get
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public virtual void Clear()
            {
                foreach (Gtk.Widget wid in __ownerControl.Children)
                    __ownerControl.Remove(wid);

                InnerList.Clear();
            }

            public int GetChildIndex(Control child) => GetChildIndex(child, true);

            public virtual int GetChildIndex(Control child, bool throwException)
            {
                int index = IndexOf(child);
                if (index == -1 && throwException)
                {
                    throw new ArgumentException("ControlNotChild");
                }

                return index;
            }

            internal virtual void SetChildIndexInternal(Control child, int newIndex)
            {

            }

            public virtual void SetChildIndex(Control child, int newIndex) => SetChildIndexInternal(child, newIndex);
        }

        internal class ArrangedElementWidget : IArrangedElement
        {
            Gtk.Widget _widget;
            internal ArrangedElementWidget(Gtk.Widget widget)
            {
                _widget = widget;
            }
            public Gtk.Widget GetWidget { get => _widget; }
            public Rectangle Bounds => throw new NotImplementedException();

            public Rectangle DisplayRectangle => throw new NotImplementedException();

            public bool ParticipatesInLayout => throw new NotImplementedException();

            public PropertyStore Properties => throw new NotImplementedException();

            public IArrangedElement Container => throw new NotImplementedException();

            public ArrangedElementCollection Children => throw new NotImplementedException();

            public ISite Site { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public event EventHandler Disposed;

            public void Dispose()
            {
                if (_widget != null)
                {
                    _widget.Dispose();
                    _widget = null;
                }
            }

            public Size GetPreferredSize(Size proposedSize)
            {
                throw new NotImplementedException();
            }

            public void PerformLayout(IArrangedElement affectedElement, string propertyName)
            {
                throw new NotImplementedException();
            }

            public void SetBounds(Rectangle bounds, BoundsSpecified specified)
            {
                throw new NotImplementedException();
            }
        }
    }
}