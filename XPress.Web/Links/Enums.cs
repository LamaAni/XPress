using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPress.Web.Links
{
    /// <summary>
    /// Lists the activation event for each of the specific script.
    /// the activation event tells the script when to be called.
    /// </summary>
    [Flags]
    public enum ActivationEvent
    {
        /// <summary>
        /// Object will be initialzied on javascript onmouseover or onfocus event.
        /// </summary>
        ActiveContext = 1,
        /// <summary>
        /// Object will be initialzied on javascript onmouseover event.
        /// </summary>
        MouseOver = 2,
        /// <summary>
        /// Object will be initialzied on javascript mouse move event.
        /// </summary>
        MouseMove = 4,
        /// <summary>
        /// Object will be initialzied on javascript onclick event.
        /// </summary>
        MouseClick = 8,
        /// <summary>
        /// Object will be initialzied on javascript onmouseup event.
        /// </summary>
        MouseUp = 16,
        /// <summary>
        /// Object will be initialzied on javascript onmousedown event.
        /// </summary>
        MouseDown = 32,
        /// <summary>
        /// Calls the object activation when the object loads into the client screen. Loads with page must be true.
        /// </summary>
        OnUpdate = 64
    }


    /// <summary>
    /// The type of the link to implement.
    /// </summary>
    public enum LinkType
    {
        /// <summary>
        /// Gloabl script links are just files downloaded as is and executed on the page. This is the only type of script that can be a head script.
        /// (compressed if in release mode)
        /// </summary>
        Script,
        /// <summary>
        /// Called depending on the object's LinkActivationEvent. Defualt is ActiveContext (OnMouseOver, OnFocus).
        /// The script is executed where the 'this' keyword points to the object the script is attached to.
        /// (compressed if in release mode)
        /// </summary>
        InitScriptFunction,
        /// <summary>
        /// Called depending on the object's LinkActivationEvent. Defualt is ActiveContext (OnMouseOver, OnFocus).
        /// Called as an object constructor that extends ($.extend(o, constructor) the current object. 
        /// If a function named '$' is present, this function is called to initialize the object and then deleted.
        /// To be defined as : {$:function(){}, ... other objects....}
        /// (compressed if in release mode)
        /// </summary>
        Constructor,
        /// <summary>
        /// Generates a style sheet file and adds the style sheet to the page.
        /// (compressed if in release mode)
        /// </summary>
        Css,
        /// <summary>
        /// Some other file to be linked to the page. (just adds the link and the url src).
        /// </summary>
        Other
    }

    /// <summary>
    /// Where the link file originates from.
    /// </summary>
    public enum LinkOrigin
    {
        /// <summary>
        /// From embedded resource, where not the url is the embedded name and the assembly is where the link is defined.
        /// Note that cross assembly links are not supported.
        /// </summary>
        Embedded,
        /// <summary>
        /// From file. Where the link is now a url. (To anywhere on the web if defined http://, or a directory relative to the current - where a file is expected)
        /// </summary>
        File,
        /// <summary>
        /// A string that is rendered onto the client source as is (href=[string]).
        /// </summary>
        AsIs
    }

    public enum LinkLoadType
    {
        /// <summary>
        /// Loads all the head scripts associated with the current control type. Can only be added via attributes.
        /// </summary>
        HeadIfPossible,
        /// <summary>
        /// A script link that will be added via the code, to the head part of the command. This link will be removed if the object demanding this link is removed from the tree,
        /// and the link is marked CanBeRemoved=true.
        /// </summary>
        Inline
    }

    /// <summary>
    /// Applies only to non script links
    /// </summary>
    public enum LinkRelation
    {
        Alternate,
        Archives,
        Author,
        Bookmark,
        External,
        First,
        Help,
        Icon,
        Last,
        License,
        Next,
        NoFollow,
        NoReferrer,
        PingBack,
        Prefetch,
        Prev,
        Search,
        Sidebar,
        Stylesheet,
        Tag,
        Up
    }
}
