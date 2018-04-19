using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Resources;

internal class SingleAssemblyComponentResourceManager : ComponentResourceManager
{
    private Type _contextTypeInfo;
    private CultureInfo _neutralResourcesCulture;

    public SingleAssemblyComponentResourceManager(Type t) : base(t)
    {
        this._contextTypeInfo = t;
    }

    private static void AddResourceSet(Hashtable localResourceSets, CultureInfo culture, ref ResourceSet rs)
    {
        lock (localResourceSets)
        {
            ResourceSet objA = (ResourceSet) localResourceSets[culture];
            if (objA != null)
            {
                if (!object.Equals(objA, rs))
                {
                    rs.Dispose();
                    rs = objA;
                }
            }
            else
            {
                localResourceSets.Add(culture, rs);
            }
        }
    }

    protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
    {
        ResourceSet rs = (ResourceSet) base.ResourceSets[culture];
        if (rs != null)
        {
            return rs;
        }
        Stream manifestResourceStream = null;
        string name = null;
        if (this._neutralResourcesCulture == null)
        {
            this._neutralResourcesCulture = ResourceManager.GetNeutralResourcesLanguage(base.MainAssembly);
        }
        if (this._neutralResourcesCulture.Equals(culture))
        {
            culture = CultureInfo.InvariantCulture;
        }
        name = this.GetResourceFileName(culture);
        manifestResourceStream = base.MainAssembly.GetManifestResourceStream(this._contextTypeInfo, name);
        if ((manifestResourceStream == null) && !culture.IsNeutralCulture)
        {
            name = this.GetResourceFileName(culture.Parent);
            manifestResourceStream = base.MainAssembly.GetManifestResourceStream(this._contextTypeInfo, name);
        }
        if (manifestResourceStream != null)
        {
            rs = new ResourceSet(manifestResourceStream);
            AddResourceSet(base.ResourceSets, culture, ref rs);
            return rs;
        }
        return base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
    }
}

