﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 输入项组，是input type="checkbox"及input type="radio"的抽象。
  /// </summary>
  public class HtmlButtonGroup : IHtmlGroupControl
  {


    private readonly HtmlInputItem[] items;
    private readonly string name;
    private readonly HtmlForm _form;



    private HtmlButtonGroup( HtmlForm form, IGrouping<string, IHtmlElement> inputGroup )
    {
      _form = form;

      name = inputGroup.Key;

      items = inputGroup.Select( e => new HtmlInputItem( this, e ) ).ToArray();
    }


    public static IEnumerable<HtmlButtonGroup> CaptureInputGroups( HtmlForm form )
    {

      var inputItems = form.Element.Find( "input[type=radio][name]", "input[type=checkbox][name]" );

      var groups = inputItems.GroupBy( item => item.Attribute( "name" ).Value() )
          .Select( item => new HtmlButtonGroup( form, item ) );

      return groups;

    }


    public string Name
    {
      get { return name; }
    }

    public bool AllowMultipleSelections
    {
      get
      {
        //如果有任何一个复选框
        if ( items.Select( i => i.Element ).Any( e => e.Attribute( "type" ).Value().Equals( "checkbox", StringComparison.InvariantCultureIgnoreCase ) ) )
          return true;

        return false;
      }
    }


    public IHtmlInputGroupItem[] Items
    {
      get { return items; }
    }


    public HtmlForm Form
    {
      get { return _form; }
    }



    private class HtmlInputItem : IHtmlInputGroupItem
    {

      public HtmlInputItem( HtmlButtonGroup group, IHtmlElement element )
      {
        _group = group;

        if ( !element.Name.Equals( "input", StringComparison.InvariantCultureIgnoreCase ) )
          throw new InvalidOperationException();

        var type = element.Attribute( "type" ).Value();

        if ( type.Equals( "radio", StringComparison.InvariantCultureIgnoreCase ) )
          radio = true;

        else if ( type.Equals( "checkbox", StringComparison.InvariantCultureIgnoreCase ) )
          radio = false;

        else
          throw new InvalidOperationException();

        if ( string.IsNullOrEmpty( element.Attribute( "name" ).Value() ) )
          throw new InvalidOperationException();

        _element = element;
      }


      private readonly bool radio;
      private readonly HtmlButtonGroup _group;
      private readonly IHtmlElement _element;


      public IHtmlElement Element
      {
        get { return _element; }

      }

      public HtmlForm Form
      {
        get { return Group.Form; }
      }


      public IHtmlGroupControl Group
      {
        get { return _group; }
      }


      public string Value
      {
        get
        {
          return Element.Attribute( "value" ).Value();
        }
        set
        {
          Element.SetAttribute( "value" ).Value( value );
        }
      }

      public bool Selected
      {
        get
        {
          return Element.Attribute( "checked" ) != null;
        }
        set
        {
          if ( value )
          {
            if ( radio )//如果是单选按钮，那么只有一个可以被选中
              _group.items.Where( item => item.radio ).ForAll( item => item.Selected = false );

            Element.SetAttribute( "checked" ).Value( "checked" );

            return;
          }


          var attribute = Element.Attribute( "checked" );
          if ( attribute != null )
            attribute.Remove();
        }
      }

      public string Text
      {
        get
        {
          var label = Group.Form.FindLabels( Element ).FirstOrDefault();
          return label.Text;
        }
      }

    }

  }
}
