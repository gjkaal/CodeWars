﻿using System.Text;

namespace AdventOfCode2023;

public static class Hash17
{
    public static int Calculate(string input)
    {
        var hash = 0;
        var characters = Encoding.ASCII.GetBytes(input); ;
        for (var i = 0; i < characters.Length; i++)
        {
            var newHash = hash + characters[i];
            hash = ((newHash << 4) + newHash) & 0xFF;
        }
        return hash;
    }
}

public struct Lens
{
    public Lens(string label, int focal)
    {
        Label = label;
        Focal = focal;
    }

    public string Label { get; private set; }
    public int Focal { get; private set; }
}

public class HashMap
{
    private const int HashMapLength = 256;
    private readonly Lens[][] hashMapFocals = new Lens[HashMapLength][];

    public HashMap()
    {
        for (int i = 0; i < HashMapLength; i++)
        {
            hashMapFocals[i] = Array.Empty<Lens>();
        }
    }

    public void InitializeHashmap(string[] instructions)
    {
        foreach (var item in instructions)
        {
            if (item.Contains('-'))
            {
                var lens = item.Split('-');
                var label = lens[0];
                RemoveLens(label);
            }
            else
            {
                var lens = item.Split('=');
                var label = lens[0];
                var focal = int.Parse(lens[1]);
                UpdateLens(label, focal);
            }
        }
    }

    private void UpdateLens(string label, int focal)
    {
        var hash = Hash17.Calculate(label);
        var lensList = hashMapFocals[hash];
        if (lensList.Any(m => m.Label == label))
        {
            var newList = new List<Lens>();
            foreach (var lens in lensList)
            {
                if (lens.Label != label)
                {
                    newList.Add(lens);
                }
                else
                {
                    newList.Add(new Lens(label, focal));
                }
            }
            hashMapFocals[hash] = newList.ToArray();
        }
        else
        {
            var newLens = new Lens(label, focal);
            hashMapFocals[hash] = lensList.Append(newLens).ToArray();
        }
    }

    private void RemoveLens(string label)
    {
        var hash = Hash17.Calculate(label);
        var lensList = hashMapFocals[hash];
        if (lensList == null || lensList.Length == 0)
        {
            return;
        }
        var newList = new List<Lens>();
        foreach (var lens in lensList)
        {
            if (lens.Label != label)
            {
                newList.Add(lens);
            }
        }
        hashMapFocals[hash] = newList.ToArray();
    }

    public Lens[] GetLensList(int v) => hashMapFocals[v];

    public long TotalFocalPower()
    {
        long totalFocus = 0;
        for (int i = 0; i < HashMapLength; i++)
        {
            var lensList = hashMapFocals[i];
            if (lensList.Length == 0)
            {
                continue;
            }

            for (var j = 0; j < lensList.Length; j++)
            {
                var lens = lensList[j];
                var focalPower = (i + 1) * (j + 1) * lens.Focal;
                totalFocus += focalPower;
            }
        }
        return totalFocus;
    }
}

[TestClass]
public class Day15
{
    private static readonly Action<string> StdOut = System.Console.WriteLine;

    [DataTestMethod]
    [DataRow("HASH", 52)]
    [DataRow("rn", 0)]
    [DataRow("cm", 0)]
    [DataRow("qp", 1)]
    [DataRow("pc", 3)]
    [DataRow("ot", 3)]
    [DataRow("ab", 3)]
    public void TestHash17(string value, int expected) => Assert.AreEqual(expected, Hash17.Calculate(value));

    [TestMethod]
    public void TestHash17ForInstructions()
    {
        var value = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";
        Assert.AreEqual(1320, Hash17InstructionSet(value));
    }

    [TestMethod]
    public void TestHash17ForSample()
    {
        var result = Hash17InstructionSet(PuzzleInput.Instructions);
        Assert.IsTrue(result < 510824);
        Assert.AreEqual(510801, result);
    }

    [TestMethod]
    public void TestInitializeHashmap()
    {
        var instructions = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7".Split(',');
        var hashMap = new HashMap();
        hashMap.InitializeHashmap(instructions);

        var lensList = hashMap.GetLensList(0);
        Assert.AreEqual(2, lensList.Length);
        Assert.AreEqual("rn", lensList[0].Label);
        Assert.AreEqual("cm", lensList[1].Label);

        lensList = hashMap.GetLensList(1);
        Assert.AreEqual(0, lensList.Length);

        lensList = hashMap.GetLensList(3);
        Assert.AreEqual(3, lensList.Length);
        Assert.AreEqual("ot", lensList[0].Label);
        Assert.AreEqual(7, lensList[0].Focal);
        Assert.AreEqual("ab", lensList[1].Label);
        Assert.AreEqual(5, lensList[1].Focal);
        Assert.AreEqual("pc", lensList[2].Label);
        Assert.AreEqual(6, lensList[2].Focal);
    }

    [TestMethod]
    public void TestHashmapTotalSampleDataFocal()
    {
        var instructions = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7".Split(',');
        var hashMap = new HashMap();
        hashMap.InitializeHashmap(instructions);
        Assert.AreEqual(145, hashMap.TotalFocalPower());
    }

    [TestMethod]
    public void TestHashmapTotalFocal()
    {
        var instructions = PuzzleInput.Instructions.Split(',');
        var hashMap = new HashMap();
        hashMap.InitializeHashmap(instructions);
        Assert.AreEqual(212763, hashMap.TotalFocalPower());
    }

    private int Hash17InstructionSet(string value)
    {
        var instructions = value.Split(',');
        var hash = 0;
        foreach (var instruction in instructions)
        {
            hash += Hash17.Calculate(instruction);
        }
        return hash;
    }

    private static class PuzzleInput
    {
        public const string Instructions = "zz=6,nh-,zdlgcf-,pvgpn=8,gbfl-,sntr-,qpt-,bm-,sv=6,qg=6,lznll=4,nb-,nbj=7,qxpl=4,qc-,mdn=8,vtz-," +
            "kf-,vrd=4,lg=6,kjb=4,gh=2,dzb-,qmg=2,dr=1,zsh=5,qjh=9,gqf-,rrh-,cdx=7,tqnx=1,zb-,jkjhc-,lbpfk=3,zz=9,pxq-,dl=4,bhp-,jpp-," +
            "nbv-,zrd=6,zpgc-,jpzvb=5,mg=7,hjlm=9,nbv-,pzj=2,pbfl-,vn-,dg=4,bppl=7,nlp-,sqnn-,vs=9,vtv-,xfbj=7,rffp-,vt=6,brsm=9,njf=9," +
            "qxd-,zmg=2,xjr-,gbfl=1,phpp-,sj-,jsj=7,sj-,ht-,hmhbn-,kn-,lqk=6,pgtvq=3,rht=1,vtz-,fgz=7,lfp-,bdn-,lft-,zx=3,kf-,zlfq=7,rdmc-," +
            "ltc=8,gqg-,mds-,xpprc=5,vxkc-,nr-,tbtq-,jh-,brsm=8,cjt=6,gxjp-,tnv-,pc=3,lqjl=8,zl=6,lch-,zpgc=4,qjh-,fbgmpx-,qnfcn=3,xmkjhp-," +
            "mq-,vdd-,lq=1,smd-,hf=1,vn=6,nhhv-,mfzxzl=9,vt-,sp-,mrv-,lldn=4,pgm-,pxq=3,hf=5,fsp=3,vld=8,nx-,fs-,zs=7,bnpj=3,phpp-,qjh-," +
            "lf=3,vld=7,qpvh=5,hsr=4,sqnn=1,zlfq=6,rtbrrv=4,fbgmpx-,pfcldf-,phnnf-,bzkrt=3,vv-,cssc=7,dxv-,mfp-,qcd-,bmmz-,jpzvb-,plk=5," +
            "tpnt-,hz=8,ckkl=8,sq=7,rdmc=3,zb-,hrp-,vv-,zk=6,jl=7,cz=5,fs-,jn-,tn=5,bmjh-,vq=3,qc-,nbv-,gfg=2,pfcldf=1,fzfll=2,vmn-,txx=9," +
            "fbgmpx-,xnph=7,vxkc-,hrp=6,tt=8,gbfl-,vrd=6,rkg=2,mxns-,bxm=1,fs-,qhj-,brsm-,fq-,qhmj=9,pgm-,njf=3,kz=6,qxd=7,bvm-,kch-,fkd-," +
            "nts=4,ghq-,hlgrxq=4,dl-,vf-,np-,qbq-,xmb=5,hl=4,hqtp=3,mtqr=7,qq-,gxxlhv=9,ccqn-,jh=4,ld=1,mkf=3,dfblmt-,smh-,qxpl-,qq=1,rskn=9," +
            "fhnsv-,vdzj-,gqg-,fsp=8,mmjv=6,dm-,qpvh=2,ltc-,dvv=1,lft-,lhb-,zlch-,pg-,bjd=6,gp=2,lg=8,gc=7,pzj-,tkkb-,vbb-,nkzqc-,xmkjhp-," +
            "hjlm-,rtbrrv-,qv-,srt=3,rkg=8,zl-,kk-,tbmrng=1,gfg=3,tk-,rgq-,sntr-,tqnx=1,zv=4,fgz-,ggls-,rgq=4,frvqh=4,sz-,dl-,qp-,mz-,vv-," +
            "gd=4,clzflm=1,rvtsrh-,jsj=6,kd=1,tt=7,jnbv=1,fbg-,mh=5,smd=9,gndn-,mdcgtq=6,rg=4,bdv-,rbzz-,gsl=5,xp-,tpnt-,kch=2,pb-,zdp=6," +
            "dmmpl-,hjlm-,sbl-,cr=7,cmmbf=6,mv=4,bggf-,cmmbf-,prfx=1,zv-,dxzn-,hms=5,jrxhs=7,qhk=9,dvkb-,jnpjz=3,drkvx=7,vs=8,drm-,bm-," +
            "rdppqb-,jk=1,gfg=1,vrd-,vp=3,tnb=9,rmx=9,qbq-,rffp-,hjq=8,fxqbdv-,hrp=7,jnpjz-,khzcd-,mfp-,vn=6,rcqx-,jn=4,ktj=2,lgcz-,bjq-," +
            "gq-,vtz-,nbv-,mdvf-,cssc-,cqn=2,lch-,mdcgtq=3,tqn-,rskn-,fz=2,ptrr=9,xbb-,hb-,rcqx-,vkh-,bhml=6,nxf-,ltc=5,cz-,zh=5,fq=3,fxb-," +
            "msh-,dr=3,plk-,xm=5,pbfl=5,ggm=9,vf-,lch=4,ljm-,pd=5,skv=2,vq-,px=5,mkx=9,vr=8,gxxlhv-,xqz=4,zlch=6,xnp-,ktg=3,zv=7,df=6,gv-," +
            "fvtbs=2,rdppqb-,gfsfrt-,pgh=2,fzrjx=9,tbtq-,mkx-,vtb=8,dxkjz-,gv-,rmx-,xjm=2,xj=2,bppl=7,jjh-,pgh=3,mg-,xqz=3,cmmbf-,llj-," +
            "gv-,xpprc=1,zz-,mfbx-,pvgpn-,hz=7,tgc-,cmlz-,qbmrv-,fc=5,cjc-,pr-,zbh-,fz=1,rjgg-,cssc=1,rmxpmp-,td=1,cmmbf-,hjlm=2,rz=3," +
            "xnph-,zlx-,mds=7,hz-,jrvx-,zl-,xj=8,jrvx-,pqgds-,hj-,pppv=4,dvv=5,hqtp-,sfnd-,cbf-,cjt=7,qg=4,sp-,fxqbdv-,xj=1,mh=6,xbb-," +
            "zpgc-,qbmrv-,dj-,lxc-,hsr-,jsj-,cpc=9,fsp=7,gqf-,qh-,dlxn-,nxf=5,cc-,fg-,gndn=2,brgd-,mxth=6,fbg-,njf-,zgf-,zjl-,llj=8,qhj=7," +
            "qllcf-,xnp=6,rmxpmp-,sbl-,ktg-,dxzn-,jpp-,blsgt=8,jl=9,cpj-,mhd=6,pb-,blqvx-,bmmz=9,bkmfj-,qpt=9,cmmbf-,mfbx=8,dp=7,lf-,qd-," +
            "tnv-,smd-,qd-,rdppqb-,vxkc=9,xfbj=1,clkv=7,jlj=2,qdb=5,clkv=6,prfx=6,zlch=3,jpzvb=3,mfp=7,gm=4,lqk-,mtgh-,cbpbs=1,xjp=3," +
            "mggjx=1,rdx=3,zk-,bzkrt-,pk=8,sp-,rb-,ggm-,gd=4,fqf=2,pg=4,xpprc=5,fxqbdv-,pfcldf-,rvtsrh-,dl-,krbj-,zx-,td=9,blsgt=4,jz-," +
            "qsrqxc-,hk-,mdn=6,fxb-,rz-,bdv=3,ll-,lmb=8,vld=2,bts=4,pfcldf-,mdn=6,fhnsv=4,zdlgcf=6,qmg-,sq=9,clzflm=5,dfx=5,lt=1,dzb=9,vg=1," +
            "plgr=8,ccqn=9,chl-,jsj-,qc-,vrd-,ds=8,sbc-,fnt-,rcqx=2,vh=5,gm-,lch=1,grzn=8,btvg-,zrd-,hrp-,xjr-,np=6,blsgt-,tn-,tqnx=3,lch-," +
            "mmjv-,dpp-,qhk=9,fsp-,bhp=8,qfh=5,clkv=1,rgh=2,vn-,rv-,ph-,pbfl=5,jh=4,ckkl-,zlx-,fzrjx=7,gfg-,fg=8,fbg-,td=9,pgh=4,bggf-,czcp-," +
            "bf-,xhlh-,rzx-,fjrxpz=3,srt=3,ttd=3,bxkk-,zvh=6,rfb-,slph-,bppl-,frvqh-,qnfcn-,bvm-,mfbx=6,qsrqxc=9,dj-,cbfq-,kg-,lzbs=7,ljvj-," +
            "qjz=9,dzb=6,lcsqz=7,fc-,jgsvq-,td-,rdx=8,rz-,cjt=3,fzfll-,sqnn-,krbj=2,dg-,zs=8,pfcldf=2,jk-,pr-,bsh=6,mfbx=2,lgcz=3,lt-,fvgn-," +
            "lcsqz=2,lrd=4,smh-,pc=3,mpn-,gc=9,kx-,jz-,bjd-,hsr=6,rmx-,mh=1,fvtbs=4,fz=5,cr=9,lzbs-,zgf-,lgq=6,qllcf=2,cm=3,bts=5,cmvp-," +
            "fnqs-,bj=3,fbg-,vbb=8,zsb=3,kch=5,ph-,bkmfj=2,sp=7,vv=3,rb=9,xjp=3,gbfl-,vxkc-,sq-,jc=1,sj-,bjmvp-,cj=9,blqvx-,fhnsv=4,pzzj-," +
            "zrh=4,rbzz=7,qv-,mh-,jz=7,nts=8,xmkjhp-,kn-,qg-,lmx-,blsgt=2,dfblmt-,kch-,gxjp=7,hb-,tqn-,gh=7,sbl-,fxqbdv-,tkkb-,tbtq=4,xqz=3," +
            "vxkc-,ktg-,rgh=7,bxnljc=9,rzx=1,mtqr-,hc=4,hd=8,tgc-,pd-,ltc-,dh-,ljm=7,kvz=8,xn-,tjt=2,ccqn=1,vdd-,qfc-,srt-,gfg=5,chc=9,sn-," +
            "pg=4,zl-,nkzqc-,sh=2,vth=1,cj=8,kf=5,nhhv=9,qkz-,zh-,gsl=9,lqjl=8,hjlm-,qpt-,pc-,frvqh-,btvg-,rd-,qs=9,vkvh-,jn=7,nt=7,jl=4,qpt-," +
            "clzflm-,jb-,qglh-,zmg=6,fxng-,jpzvb=6,fbc=8,vbb=3,gxxlhv-,lt-,tzndx=2,plgr-,fz-,zmg-,gkr=4,zs=4,mggjx=7,ch=9,fgz=3,zdp=1,dlxn-," +
            "cksg=6,nmc-,tbtq-,fsh-,msh-,gd=7,pjd-,xd-,gc=6,jz=5,vxl=5,rdmc-,zbh-,vdd-,vf=7,rgh=9,kch-,fn=1,rmx-,jc=2,dx=4,pb-,skg=6,fnqs=6," +
            "fcgv=8,lqjl=2,qfh=2,cz-,clkv=1,pbfl=9,vk=3,bjd=9,ht=7,zs=3,dvv=7,njf-,dbtf=9,mg-,fn=5,jlj=7,jrvx=3,tkkb-,qxd-,rgq=4,prfx-,vfq-," +
            "cj=2,ptrr=5,mpn=6,dlxn-,brsm-,rs=5,zh=6,qxd-,plk=7,xc-,qxpl=3,qqdv=4,lhb-,mpn=9,mrv-,vt-,dqq-,qqdk=3,lfp-,lqk-,ph-,ljvj=3,xn-," +
            "gsf=8,fzfll-,vxkc=7,pt-,mggjx-,kf=3,qxft=5,jn=5,pqgds-,kch=4,ld=4,dvv-,bm-,dxv=4,ks=5,pcv=6,bjmvp-,ghq=2,rd=5,fxqbdv-,tnv=7," +
            "jb=1,tk=1,xmn=7,qkz=7,xhlh=9,ph=2,zlfq=8,xjp=5,vxl-,mrv-,psqnq-,qgbcv=9,bts-,mj-,vld-,mz-,zgf-,vld-,cmvp=2,gv-,dfx=8,qbfmk=7," +
            "vkvh-,gp=8,ltc-,blsgt=5,sbl-,rjgg-,nm=4,dm-,hg=3,qq-,bts-,mmjv-,rgr-,rb-,msh-,ddzvp-,qfc-,pk=2,vrd-,hrl-,zl=7,pzzj=5,rxjl-," +
            "bxnljc-,xj-,zlch=2,dzccv=4,qv=8,hkh=3,kl-,fnqs-,pd-,fkm=7,dj-,zdvmh=7,qxpl=6,vrd=4,bhml=4,pt-,mtqr=8,hkh=3,txx-,kz-,dpp=5,sz=9," +
            "vth=3,xpq-,bhp-,np=1,zmg=9,lf-,bmmz-,gsf-,gd-,pk=8,rxjl=2,mdn=8,pk-,qkz=9,xfbj-,pbfl-,fn-,jpzvb=7,jn=9,hf-,btvg-,brgd-,vk=1," +
            "mrv=6,dqq-,npm-,pk-,frvqh=3,vk-,blsgt-,sq-,kg-,mdvf=7,vs-,bqzn-,hk=4,vfc-,dx=6,hk=5,cmlz-,qgbcv-,mrv=3,mds-,nts-,fbgmpx-,qhk=1," +
            "fjrxpz=2,ns-,rjgg=4,fsh=8,pr=7,hjlm-,fs-,dkv-,pqgds-,kch=9,dpf-,hf=6,hrp-,fgz-,czcp-,rv=7,hg-,ht-,llm=2,lxc-,ggls-,xjp-,fhnsv-," +
            "qv=6,cmmbf-,zh=6,xmkjhp=7,smh-,fcx-,bjd=9,smd-,fsp-,qp-,bzkrt-,mx=3,xn=5,vkvh=3,xfbj-,bf=3,dfx=6,zpgc-,fs=5,mds=9,gc-,dg-,nxf-," +
            "rjgg-,nts=3,bhml-,zlch-,xn-,dkv=1,pjd=2,pppv-,bv-,bv-,vr-,jrvx-,khzcd-,pfcldf=3,vc=9,vfq-,dr-,mx=1,ch-,gtv=9,pzj=1,dlxn-,dmmpl-," +
            "cm=4,qbq=3,hqtp=2,cbf=1,ckkl=7,hsr-,fnt-,xnp-,bzzb-,lqjl-,fvtbs=7,zrh=2,zp-,zdp-,szrt-,qmg-,dpf-,dhz=9,dh-,tzndx=4,zb=5,lgq=8," +
            "prfx-,jnbv=2,pxq=5,bdx=7,bvm-,cpp-,dbtf-,hkh=1,szrt=7,ds=9,cmmbf=6,jb-,nts=5,fbg=4,zpgc-,hkh=5,fc=4,fnqs-,nxf=6,lg-,mds-,txk-,px-," +
            "nsz-,rfb-,dbtf=4,fcx-,qtqd=1,bv=8,lhb=2,ggm=5,vlg-,rs=6,bdn=8,tf=7,mv=7,zlch=1,cm=6,gkl-,bsh-,zsb=9,gvlz=9,qjhc=8,gkr-,lgq-,qjz-," +
            "txx-,rz=9,mds-,qhk-,sxzxpp-,vmn-,smd=5,lq=5,vr=1,fcx-,mv=8,dp=1,rd-,vk-,cbfq=7,bsh=4,prqfc-,cm-,ds=2,bjd=9,bkmfj-,fq-,fc-,lbpfk=9," +
            "nmc-,zpgc-,xmn=5,dm-,cmmbf=2,cmlz=5,zz-,smcrjx-,nxf=5,rg-,gzxqx=7,hf-,pgtvq-,fc=3,bsb=1,vkvh=6,bnpj=5,qpvh-,fqf=9,dvkb-,kd=8," +
            "ch-,vg-,bxnljc=7,sx=5,gvlz-,dd-,zs=7,qgbcv=7,lznll-,sh=5,jrxhs-,frvqh=3,jsj=4,cmvp=2,cbfq-,blsgt-,llj=8,txx=9,frvqh-,lbpfk-,chl-," +
            "gq=8,bjq=8,xmkjhp=5,cv-,pr-,tvzr=6,pd=7,fsp=8,cbfq-,dh=3,pbfl-,kg-,rfb=5,ch-,dzccv-,dkhtfp-,hkh-,fs-,dml-,mfzxzl=1,prqfc-,dkv-,zb=9," +
            "hg=7,hmhbn=2,lcsqz=4,smh-,fz-,rzx=6,bppl-,jb=4,fsp=9,bmmz=3,bng=9,gfsfrt-,rdmc-,bppl=4,bqzn-,hkh=6,hqtp-,dhz=2,nts-,srt=3,pr=9,nsz=7," +
            "gq=3,fbgmpx-,gq=4,tbmrng=7,pzj-,fsp=6,vh-,ks-,rht=2,qmg=9,dp=7,zmg=2,fxng=7,lldn=9,ggm=8,dj-,nxf=2,dzb-,nb-,tqn=8,qjz=4,gfg=9,pr-," +
            "rkp=9,qhj=6,dml=8,xqz-,bv-,cqn=2,lft=8,bf=7,brsm-,cmlz=9,qfc-,gdktn=2,xmb-,fg=9,ghq-,rdx=2,lz-,pgm-,rv-,cjc-,txk=3,zqr=5,hg-,jn=8," +
            "hj-,dj-,qfc-,tk-,grzn-,qbfmk=8,phpp=1,nt-,ljm-,ll-,qddb=5,qhj-,fz=6,sn=8,pgh=3,dvv=7,bv-,qd=1,hz-,ljvj-,lft=6,brsm-,zgf=6,njf-," +
            "jkjhc=3,dx=1,sntr=7,vr-,dvkb=6,cz=4,fkm-,txk=1,gm-,phpp-,rrh=2,dbtf=1,mkx-,qxpl=2,rgq=6,xm-,tkkb-,vlg=3,cjc-,rbzz=9,lt=9,kd=7,ds-," +
            "tnb-,vbb-,pxq-,clkv=4,tg=7,fhnsv=8,rffp=1,bgq-,brsm=7,lzbs=5,lqjl-,tpnt=9,zn-,jx-,fc-,mg-,krbj-,cdx=5,cs-,rfb=9,jkjhc=6,zv=2,gp=6," +
            "hkh-,ph-,cdx=9,nb=9,pbfl=2,qcd=6,qgbcv-,kl=1,llj-,pd-,cmvp=4,xdjxq=7,dh=9,lrd=6,fg-,cz-,rz-,gdktn=4,gsl-,qhj=3,mfp-,cfm=4,dvv-," +
            "blsgt-,zcn=7,dxzn-,xmn-,qq-,gp=3,bvm-,tf=2,dg-,ktj=3,xhlh-,qgbcv-,vr=3,lg-,dd=8,zbh-,khzcd-,qd-,rd=8,xmkjhp-,prqfc=2,bnpj-,cz-,vld-," +
            "rskn=8,mg=1,phnnf=3,dj-,blsgt=3,fbc=4,ggls-,zjl-,chl-,fvgn=4,grzn-,sh-,vg=6,xdjxq-,cmlz=1,mxns-,kk-,nlp-,cj=8,fdv=2,xqz=9,pbfl=5," +
            "qh=4,hjlm=2,fzrjx-,zx=8,vfq=5,fvtbs=9,lft-,jh-,bxm-,tqn=8,hkh-,fbc=9,bgq=7,gxxlhv=7,qd-,fkd=8,sx-,fxb=1,skg=7,qc=6,nh=9,mdvf=7,lcsqz=5," +
            "mggjl=9,qgbcv=6,cpj=6,hms-,kz-,ljvj-,jrvx=8,ph=7,jc-,ch-,qqdv=3,blsgt=2,dg=2,krbj=5,lg=2,vkh-,qjhc-,ddzvp-,fxb-,sbc=4,dbtf-,xn=2," +
            "gxjp-,lhb-,lg=7,vk-,gqf-,rxjl=7,cqn-,dd=2,hd-,zn=5,xmkjhp=6,qxd=7,qbmrv-,lqk-,phnnf-,npm-,bts=2,gqf-,hj=8,bzzb=8,ptrr=3,sz-,npm-," +
            "nbv-,jb-,qd-,kvs=7,cs=7,bm-,sbl-,lf=2,dml-,rfb=3,dfblmt=9,fcx-,fzfll=9,dfx=3,sbc=3,kjb=9,hms-,gq-,tgc=6,vh-,fnl-,zlfq-,qhk=8,xbb=3," +
            "ljm-,mggjx-,zjl-,blqvx-,sxzxpp=2,qbfmk-,rdx=7,cjc=8,pbfl-,dpf-,rmx-,xnph-,ljvj-,dml=6,qbmrv-,lhb=5,rzx-,dl=5,rgq-,fz-,dqq=6,gfg=8," +
            "ggm=9,ns-,mq=1,dzb-,clkv=2,hjq-,kvz=7,gfg-,clzflm=5,mkf-,bkmfj-,xn=4,xk=3,lrz=9,jx-,mv=5,rskn=4,cksg-,zbh=2,hf-,grzn=5,bts-,xm-," +
            "fxqbdv=4,qddb-,gs=7,qgbcv-,qhk=5,vtb=5,zdlgcf=2,vhmc-,lg-,sq-,vdd=2,nhhv=4,bsh=6,drm-,mg-,vkvh-,dxzn=5,dbtf=2,slph-,kvz-,hjq-,xj=3," +
            "cmlz-,nlp-,vld=6,jn-,gbfl-,bmmz=9,gfg-,ktj-,rfb=4,sq-,cjc=5,zh=6,bhml-,jlj-,rzx=4,xp-,fvtbs=5,hd=6,pd=3,zb=7,nx-,zsb-,qhk-,gdktn=5," +
            "xmb=2,qqdv-,bzzb-,pr=9,rtbrrv=8,hrl=9,hf-,nh-,sz-,mj-,rbzz=9,fnl=1,ckkl-,bg-,fjrxpz-,pgtvq-,llm=7,rvtsrh=2,qh=8,blqvx=6,nbv=1," +
            "mdcgtq-,kl-,tbmrng=1,mj=5,fnt=7,qgbcv=1,fzfll-,zsb=6,fxng-,hj=4,hj=9,hg-,hrl-,mj-,qglh=8,pgtvq-,cs-,btvg-,lzbs-,lz=9,rtck-,gsf=5," +
            "mkf-,lrz=2,xjm=2,xn-,xnp-,xqz=8,vdd=3,zv-,bhp-,qfh=4,blsgt=8,drm-,czl-,bng=2,rmxpmp=8,lt-,xnp-,pd-,llj-,nbv-,xj-,qbmrv=6,xbb-,lln-," +
            "hkh=3,sx=4,zdp=5,gxjp-,hqtp-,cbf=4,fbgmpx-,vdd=2,zlfq=3,smd=2,fxb=9,chl-,xpq-,fz-,bjd-,krbj-,vq-,fqf=6,gq=5,gqf-,vq-,jnbv-,nmc=9," +
            "gxjp=6,fz=3,qq-,mmjv-,gdktn=3,vf-,xd-,xd=2,gs-,fsp-,btvg-,tf=9,llm-,ghq-,tpnt-,sbl=4,qfh-,xd=3,mpn-,rht-,gvlz=2,gsf=3,mtgh-,qqdk=3," +
            "gv-,pt-,ltc=8,cz=2,fnt=4,gs-,lq-,rhh-,rmxpmp=9,vbb-,bdv-,fzrjx=3,bppl=5,fg=6,fjrxpz=3,hk=4,xn-,qbfmk-,xjm-,zjr-,bgq-,fs-,bv=4,lbpfk=4," +
            "vlg=8,rtck-,sqnn=1,hrp-,vdzj=4,bzzb=1,lg-,qhj=5,zgf=3,tnb=7,pk-,ht-,jnpjz=3,tg=9,qnfcn-,qjh-,qxd-,xqz=6,mdcgtq-,gdktn=6,tgc-,pgtvq-," +
            "jn=5,cbf-,vdd=3,qfc=8,pbfl=1,lln-,fxng-,dq=5,pgh-,vh=7,lft=3,xdjxq-,czcp-,gkr-,btvg-,rv-,tt=1,ddzvp=2,ccqn-,vbb=4,pc-,sq=1,fkm-,mfp=1" +
            ",mx-,jn-,fnt=4,ht=3,qgbcv-,jpzvb-,fkm=3,slph=7,qsrqxc=2,bvm=3,mx=8,hd=9,hg=1,dm=2,mv=9,xjm-,dqq-,tjt-,td=3,bgq-,pvt-,lg-,cmvp=8,kx-," +
            "vfq=9,gsl-,ht-,qcd=3,qxd=4,bggf=1,gp=8,bm=9,mv-,sq-,jsj=4,ljm=2,qmg=3,pppv-,qbmrv-,jk=3,vrd-,cj=8,mfzxzl=7,dxv-,rknn-,qhmj-,kvz-,lch-," +
            "krbj-,ld=5,xnp-,grzn=1,jgsvq=1,sqnn-,tt-,fsp-,nh-,zjr-,mg-,qhj=4,lln-,xc=9,xnph-,cqn-,rrb-,lrd-,pfcldf=1,hrl-,dlxn=6,tqnx=9,zpgc=6," +
            "qcd=7,vrd-,mtgh-,fdv-,hmhbn=2,xk-,vfc-,zlfq=6,bsb=8,njf-,qxd=2,df=9,fcx=3,zx=8,gndn-,gzxqx=9,fvtbs=8,fc=2,fn-,tt=3,rht-,rv-,ff-,dpf=5," +
            "qtqd-,qsrqxc-,zlch-,qq=3,rgq=6,pzj-,lhb-,zdvmh=5,df-,fz=9,np=9,kvz=7,lf=6,sz=8,hl=3,qfc-,nt-,rdmc=2,lft-,fcgv-,pr-,qllcf-,mh=6,smd-," +
            "gc-,qd=4,dpf-,lzbs=7,qbmrv=5,pd-,rgr=1,zn-,bkmfj=7,ggm-,llj=1,qllcf=4,tbmrng-,hrp=4,fgz=4,qxft=5,btvg-,xdjxq=2,ktj-,fnqs=4,tzndx-," +
            "rhh=9,zs=5,pd=2,czl=4,jnpjz-,kjb=7,ltc-,sxzxpp=6,qh=3,gzxqx=4,bdx-,lldn-,rm-,drkvx=6,gh-,lldn=2,bvm=4,mggjx-,tbmrng-,zsh=4,nxf-,nr-," +
            "qbmrv=3,npm-,tcrtbl-,hj-,dq=6,fzrjx-,qjh-,pt=4,dm=5,xdjxq=5,hb-,zbh=3,sfnd=4,fbgmpx-,qnfcn=4,sq-,btvg-,lrz-,bf=2,vdd-,pxq=7,mds=5," +
            "qxd-,kjb=6,skg-,kjb-,mfzxzl-,bf=8,zpgc=7,rkp=8,jjh-,xmb=9,bv-,dmmpl-,ggm=4,pbfl=6,llm-,rjgg-,fxqbdv=1,ld-,fqf-,xhlh-,zmg=1,lqk-,sqnn=5," +
            "tf=7,qxd=1,kxcbj-,sxzxpp-,pb-,jkjhc=1,cksg-,tqnx=8,bzkrt=7,rb-,mv=1,fq=2,lxc-,cc-,qxpl=5,hc=1,gqg-,gbfl-,kx=4,lmx=8,qs-,cpj-,xjr=4," +
            "pjd-,gfsfrt-,qv=5,czl=9,fxng=5,rdx=5,tnb-,dm-,gq=1,fdv=3,bng=5,vr=5,zp=6,cjt=6,xk-,cbpbs-,cbpbs=4,drm-,blsgt=5,cpp-,cpj=9,mdvf-,xk-," +
            "mg=3,bdn-,lhb=7,ddzvp-,cr-,ptrr=8,ghq=1,hz-,pfcldf=5,smd-,nmc-,cbfq-,kvz-,vhmc=9,hrl-,lgq=7,smcrjx-,jn=3,clkv-,fg=5,xmkjhp-,ghq=5," +
            "pfcldf=9,ds=9,gs=6,hlgrxq-,qkz-,zdvmh=4,pppv-,vfq-,pfcldf-,jlj-,pg=1,zrbzg-,lch=5,jn-,prqfc=7,zbh=8,qqdk-,htn=2,xqz-,cksg=8,dkv-," +
            "nkzqc=2,hf=3,vlg-,nhhv-,lgq-,kl-,hkh-,sqnn-,fbc=8,dq=1,hz-,xbb-,fdv-,bts=1,fbc=4,fzfll-,zp-,vg-,lrd=6,mhd=8,rdppqb=1,qg-,fs-,cxh=1," +
            "zgf=6,rgr=2,tqnx-,vkvh-,zgf=6,xdjxq=8,pt-,dg-,xm-,pc=8,llj=8,zlfq=4,mggjl=2,vn=2,xjr-,pqgds-,bggf-,prfx-,fnqs=1,mfbx-,zz-,qg=3,pzzj-," +
            "gkr=9,dpf-,ccqn-,mds-,cs-,jpp=3,kf-,gbfl-,bsh-,njf=7,mj-,vtb-,fzrjx=6,zlch-,lldn-,vkh-,fs-,bxkk=9,nbv-,mkx=5,vtv=5,tnv=5,gbfl-," +
            "smcrjx=3,xpq=2,tn=4,blsgt=2,ggm-,dpp=5,fbc-,dzb-,npm=7,qhmj-,vt=1,dvv-,slph-,jrvx-,njf-,cj=2,sz=8,clkv=7,rbzz-,blsgt-,dml-,nhhv=2," +
            "hkh-,qkz=8,dxzn-,fs-,jn=1,xdjxq=6,ccqn-,dj=1,fvtbs=7,pvgpn-,ll-,pcv-,rkp=6,bxkk-,fzrjx-,fzrjx-,jb-,chl=3,gkr-,llm-,cksg-,jpp-,sntr-," +
            "nb=7,hrl=7,kd-,qdb=4,mggjl=3,lq=9,hrl-,vlg-,ks-,jk=7,fnt=6,gvlz-,bzkrt-,qpvh=3,mdvrdj-,hj-,zbh=6,dkv=7,mrv-,mtqr=6,hlgrxq=7,pxq-,lrd-," +
            "phnnf-,qfc=9,jz=3,df=8,dfblmt-,ld-,xnp-,hz-,bnpj-,plgr-,qs=8,bdv=2,mggjx=7,qh=3,mx=2,td-,fnt-,ckkl-,bf=4,dpf-,xjp=4,phpp-,tkkb=5,qxpl-," +
            "trcm=8,mfbx=1,bxm=6,lbpfk=1,fxng=4,skv=5,vlg-,gbfl=4,dml=9,zlfq=8,rg-,qdb=3,rgh=6,ds-,nh-,rdx=7,xpq-,fxb=4,ljm-,dj-,npm=8,fvgn-," +
            "szrt-,tqn-,cdx-,nbj-,hb=2,qh=5,vg-,nlp-,rdppqb-,qv-,phnnf=6,fg-,tpnt=1,dbtf-,cj=7,drkvx-,ktg-,ddzvp=7,zz=4,zgf-,gkr-,jx=7,nbv-," +
            "prqfc-,mpn-,brgd-,tbtq-,lldn=8,bzkrt-,mg-,gkr-,jz=4,fhnsv=3,zdvmh-,tnv-,zlfq=9,ph=2,bzkrt-,rcqx-,fsp=3,rgr=6,tqn=2,nkzqc-,gxxlhv=9," +
            "rkp=3,dm=4,lq=5,vk-,qhk=8,jnpjz=3,ks=1,ns=5,fsp-,qbq=8,cs=8,xnph-,qbmrv=2,pcv-,hrp-,qcd=2,dfblmt-,bmjh-,qv=3,sntr=9,pjd=3,hf=9,hms-," +
            "vq-,kd=1,rgh=3,kf=1,mfbx=6,nm=6,dlxn=4,rffp-,lxc-,chl-,txk=2,qs=5,dhz-,ccqn-,vxl=3,zdvmh=7,rg-,fg=5,zpgc=3,vfc=8,vkvh-,mv-,npm=1," +
            "vbb=4,rm=6,rb-,tk-,xmb-,ghq-,jk-,xjp=6,lrz=1,df-,lz-,rv=8,qqdk=9,cz=4,mdvrdj-,tf-,txk-,zjl-,rd=2,cfm-,zs=2,bjmvp-,sn-,pj=1,zqr-,tf-," +
            "pr-,bhp=6,kch-,jrvx-,hrp=7,blsgt-,jx=1,dx-,zgf=7,jrvx=3,zh-,mv=4,ckkl=6,df-,pd-,xbb-,dh-,cmvp=4,zh=4,zl=2,sqnn=1,gdktn=1,vxl=2,rfb=5," +
            "brsm-,cksg-,dml-,tzndx-,lmx=6,lz-,nb-,ckkl=5,lq=8,vmn=1,fgz=8,nt-,lmx-,cmmbf-,sbl=4,rm=3,ljm-,dbtf-,fzrjx-,llm-,msh=9,bxnljc=8,dh=7," +
            "pgtvq-,ltc=8,fbc-,gxxlhv-,gndn=2,cfm-,gsl-,dg-,bmmz-,cdx-,dzb=1,xpq-,rjgg=8,cqn-,qqdv-,rbzz-,lzbs=5,rkp-,kvs-,zn=9,dxkjz-,cc=7,mfbx=1," +
            "tqn-,mhd-,xpq-,mj=2,zs-,nr=8,zdvmh=5,qkz-,sn=4,lft-,hrl=9,zrbzg-,cr-,qbmrv-,cmmbf=6,slph=8,xz-,txk=7,sq-,msh-,hqtp=4,xdjxq=8,cbf=2," +
            "pjd-,xmkjhp=1,fkd-,bjd=4,ptrr-,gq=8,mz-,nbj-,ddzvp=3,zmg=5,rcqx=9,skg=6,xd=9,zsb=7,sj=6,pgh=2,gfsfrt-,zbh=7,gdktn=9,llj=6,qg=9,ch-," +
            "lhb=1,mj-,hms=1,cmvp-,vv-,lft=7,zh=6,qgbcv-,qhj-,mdn=5,rknn=1,kxcbj=2,zlfq-,vrd=8,htn-,nbj=1,bnk-,ktg-,lzbs-,sk=5,vkvh=8,fsh=7,fgz-," +
            "fkm=5,pg-,sbc-,gndn=7,qbmrv=2,htn-,zmg=7,pvgpn-,jnpjz=1,rxjl=9,kch-,gkr-,lz-,pgm=3,sntr=2,gqg=4,fxqbdv-,hlgrxq=9,dfx=5,gqf=1,cxh-,xp-," +
            "rz=3,tt=6,tbmrng-,mdn=1,fxqbdv-,sqnn-,zdp-,tpvdx-,qs=7,sv=6,pvt=7,zdvmh-,rtck-,ghq-,tvzr-,grzn-,gsl-,tpnt=6,vt-,fnl-,fg-,ktj-,rrb=6," +
            "vld=1,nsz-,pppv=7,blqvx=2,qcd-,srt=7,zn-,fxb=5,xmkjhp-,mpn=9,xmb-,mdvf=6,hf-,sr=4,khzcd-,xb=3,mfbx-,lhb=1,zlx-,tf-,xb=3,fjrxpz=5,bm=5," +
            "vc-,kd=1,mx-,rgr-,sxzxpp-,dhz=3,lmx=6,vlg-,dl=1,xdjxq=5,dxkjz=2,fjrxpz=2,gkr=9,bxm-,qglh-,mtgh-,pxq-,vbb-,lqjl=4,pgm=1,rdx-,ghq=3," +
            "mmjv=8,bhp=8,blqvx-,tn-,xj-,lhb-,bppl-,fkd=9,zjl=3,gvlz=2,fdv-,zp-,fxng=9,pc=3,bts-,gtv=4,pvgpn-,ll=3,rkp-,prfx-,zrd-,hlgrxq=3,rgq=7," +
            "pppv-,tt=1,qqdk-,szrt=9,nts=1,xd-,gqg=4,hd=3,gndn=4,qfc-,ld=1,fn=5,gbfl=3,gm-,qhmj=3,tkkb=7,lmb-,fq=3,qsrqxc-,vf=1,bjd=5,bkmfj=5," +
            "qbmrv=5,hlgrxq-,bdx-,pvt=5,ns-,qfh-,clkv-,qnfcn=9,hkh-,hd=1,vtz-,ggls=9,rv=7,zrd-,fxqbdv-,mq-,jgsvq=5,lfp-,fcx-,lz=8,bzzb=8,xn-,nts-," +
            "bjd-,jk-,chc=1,chc-,vs-,lfp-,kk-,jx=8,npm=4,vg=7,lq-,pxq=9,bmmz=9,fn-,ttd=8,qmg-,jc-,qpvh-,pfcldf-,dp-,gsl=2,smd=9,bgq-,lqk=2,ckkl=1," +
            "ckkl-,bggf=8,vg=8,bf-,bjq=4,czcp=3,xnph-,rg=6,jz=8,fkm-,qbq-,dg-,fxb-,vlg=9,kf-,vmn-,bjmvp-,mh=1,rrb-,bdn=7,rzx=2,rgh=7,phnnf-,llj=8," +
            "tgc-,chl-,ch=8,gqg=8,qc=5,qh=7,rskn=7,nlp=6,cjt=1,hrp=1,cbfq=3,cmmbf-,hl=1,fz-,ddzvp=2,bjd-,smh-,jl=3,rgr-,tg-,bkmfj=8,sxzxpp=2,ph-," +
            "vp=4,kl=1,bnpj=6,kf-,vc=6,zp-,hb-,ghq=1,lznll=5,hl-,dkhtfp=7,lbpfk-,bxkk-,tqn=7,vfc-,vrd-,qjhc-,nxf=1,cjt-,hg=3,zrbzg=6,chl-,phpp=3," +
            "ktg-,hg-,xnp=2,xd=2,df=6,jz-,vtv-,cmvp=6,fdv-,tzndx-,bjq-,gqf-,zlch=6,cc=2,kg=4,grzn=9,mtgh-,xjr=3,tkkb=3,zp-,tk-,xd-,dj-,td=2,gkl=1," +
            "tt-,sq-,dr-,lrz-,qqdk=6,dpp-,jc=7,nbv-,mfzxzl-,brgd-,zb-,vv-,fxb-,mdcgtq-,rkg=6,rrh-,kd=3,zjr-,xb-,dml=1,smd-,prfx=1,rbzz=2,zrbzg-," +
            "xdjxq=8,bnk=1,cr=5,zlch-,fcgv=6,ggls=1,mtqr=2,dh=7,qxd-,zb=7,fqf-,bnk-,gxxlhv-,pbfl-,pzzj=6,pj=5,zrd-,xqz-,nlp=5,pppv-,tjt-,psqnq=2," +
            "vh=3,vxl=3,pzj-,zb=7,rfq=5,hmhbn=4,vlg=2,lhb-,rv=8,sxzxpp=3,hmhbn=9,bf=2,cj=7,vk=4,zsb-,brgd=3,pbfl=2,xz=3,rv-,plgr=5,tn-,htn=9,rcqx-," +
            "rm=8,bqzn-,cz=5,xnph-,jc-,rdmc=5,gfsfrt-,pzzj-,hjlm-,bvm=9,lgq-,dzccv=6,rmx=4,ccqn-,vxkc-,gqf-,pgtvq=2,ht=4,gqg-,mtgh=7,hrp=4,bnk=3," +
            "pgm=7,mmjv-,hms-,jb-,lt=7,xb=1,jrxhs-,drkvx-,qbmrv=8,mj-,zh=2,nb-,nb-,hz=1,vk=9,gc=5,mkx=4,sxzxpp=2,qddb-,jnbv-,rb=4,tjt=1,pj-,dpf-," +
            "ktj=3,tn-,plgr=9,xmb=9,bnpj-,hmhbn=7,xqz=4,sk=2,qjz=9,cbpbs=8,szrt-,nx-,chc=7,bxnljc-,ph=3,zh-,rdppqb-,lz-,mfp-,rgq=6,qfh-,bv=7,qd=7,nt-," +
            "qc=8,grzn-,fn-,vrd-,hlgrxq-,vhmc-,lft-,fnt=4,fkd=3,zdvmh-,hb=3,chc-,npm-,qfh-,qxpl=5,kg=4,vfc-,kvs=8,qbmrv-,bdv=4,sp-,qs-,vr=9,nlp=9," +
            "gxjp-,tf=4,ckkl-,pc=1,hmhbn=5,rdx-,gh=9,tn-,jpp-,lz-,sp-,zjr=2,nlp=7,frvqh-,rz=9,dvv-,gh-,prqfc=1,fjrxpz-,ktg-,fnt=2,nxf=3,pvgpn=1,mhd-," +
            "dd-,xc-,sz-,fn-,np-,rxjl-,pr-,bhp-,vbb-,rz-,zdlgcf=3,cbf=3,rgq-,cc=6,cpc=3,cpp=7,ks=3,fzfll=4,jsj-,pjd=6,szrt-,fgz=7,nh-,pjd-,lxc=3," +
            "qxft-,bsh-,bf-,hd-,bsb-,cr-,qqdk-,rmxpmp-,zmg-,sn=8,cc=7,pqgds-,xpq-,jkjhc=5,vf-,kn=4,mggjx-,drm-,vmn=2,btvg=8,ptrr=9,rdmc=6,gfsfrt=5," +
            "sfnd-,gndn=8,rs=7,bxm=7,sbl=9,zrh=1,qbmrv=9,pb-,lldn=7,sr=3,zrd-,qkz-,vld=1,dm=6,jgsvq-,rg-,sntr-,rknn=1,pfcldf-,zl=4,fz=1,dp-,pc-," +
            "fcx=7,nr-,xmkjhp=3,dhz-,lf=2,cc=2,nr-,gqg=6,rrb-,cmvp=8,gfsfrt=7,mxth=2,pb-,xm-,bzkrt=2,tpnt-,vlg=7,xfbj-,sp-,qjhc-,prqfc=6,bhp-,msh=6," +
            "zjr=6,jh-,bdn-,dvv-,qnfcn=8,mfp-,zsh-,fcgv-,fg-,drkvx=1,qkz-,jnbv=4,bvm=5,kn=8,ltc=4,qsrqxc=6,df=2,dvkb=4,vfc-,qhmj=1,ggls-,vr-,hjq=5," +
            "jn=2,zcn-,ks-,jnbv=5,rffp-,skv-,pc-,hc-,zlx=6,zlfq=9,fcgv-,rjgg=7,xz-,cxh=7,lzbs=5,dg=8,fzrjx=7,cpp-,rmx-,cxh=7,ljvj-,bdv=1,sj=5,rjgg-," +
            "hc-,sntr-,rrh-,szrt=5,vf=5,zdp=6,kvz=8,smd-,xfbj-,dpp=9,vmn-,sx-,qxd-,kd=5,dfblmt-,gqf-,pgtvq-,lmb=7,szrt=4,qbmrv=8,nlp=5,ktj-,xb=5,xb-," +
            "sp-,vbb=3,bzkrt-,nsz-,gdktn-,smd=9,vfq-,tf-,dx=7,gdktn-,kk=5,tqn-,nb=4,pgh=9,fxng-,rfq=5,lxc=3,cpp-,qtqd=7,ptrr-,pgm=5,lq-,drkvx=9," +
            "btvg=7,clkv=6,rkp=5,tbtq-,drm=1,gp=4,dkv=7,cr-,kz-,xp-,xmkjhp-,sntr=9,mg-,bvm=5,gxjp-,qqdv=5,kx=5,cmmbf-,pr=1,xbb-,qllcf-,jx-,frvqh=8," +
            "pk=9,xm=3,hrp=2,sq=8,zbh=4,qjz-,zn-,xpprc-,pgtvq=3,mggjx-,mpn=6,tzndx-,zk-,rfb-,bqzn=8,kjb=2,dxkjz-,qc-,np-,pzzj=3,dxzn=1,vdzj=2,lf=6," +
            "zrbzg-,drkvx=5,bzzb-,hl-,hrp=6,zlx-,xpq-,pr=6,cmlz-,tpvdx=8,lmx=4,chl-,vlg-,mv-,chc=9,dxkjz=5,cdx=3,lqk=2,kch=1,qmg-,rrh-,hjlm=8,vrd=4," +
            "rdppqb-,vp-,hf-,zn=2,xmkjhp-,qg-,bzzb-,gq=2,sz-,rbzz-,htn-,dxv-,drkvx-,hjlm-,qhmj=2,zmg-,hlgrxq-,vld-,mfbx=7,dvv-,fcx=2,fjrxpz=6,xqz-," +
            "ghq=1,qpvh=4,cs=3,tgc-,ttd=6,zsb=8,zh-,qgbcv=5,prfx-,htn-,zz=6,xbb=9,dxv=9,xqz=9,qxpl=9,dh=7,kd=8,fq=8,vc-,bts-,bhml-,zp=2,mq-,dm=1," +
            "phpp-,txx=2,qqdk=8,xfbj-,dx=5,prqfc=9,vfc=8,sbc=4,mtqr=6,cssc=6,dfx=8,gxjp=3,rmxpmp=9,vtv-,kf=3,mpn=3,fhnsv=2,rmx=3,gs-,smcrjx-,kn=3," +
            "gtv-,rdppqb-,tpvdx=7,nbj-,gq=9,gsf-,frvqh=6,gtv-,mfp=8,qqdk=8,zrh=6,pg=2,fkm=7,dmmpl-,dr=2,mds=8,rvtsrh-,dkv=4,vtb=7,rmxpmp=2,bjd=2," +
            "bppl=6,vlg-,dpp=4,gfg-,bsh=4,smd-,pgm-,dx-,fz-,vmn-,nkzqc=2,slph-,fhnsv-,rgr-,lz=9,qv=4,kd-,nx=8,kk=8,cpc=2,fbg-,gqf=9,fg-,dzccv-,qhmj=1," +
            "ddzvp-,lrd-,llm=8,qnfcn-,gtv-,vxkc=7,bm-,mrv-,xjp=4,ff-,skv-,qfc-,pgh-,rxjl-,grzn=7,rkp=5,vk-,rskn-,rmxpmp-,tqnx-,fcx=4,phpp-,chl-,zrd-," +
            "jnpjz-,lcsqz=8,cz-,lhb-,lg=9,vdd-,brsm=8,fc-,zsh=1,xpprc=4,jpp=2,dmmpl-,fsh-,gsf=3,gndn=2,sq-,xjm-,lf=4,ht=3,pjd=9,btvg-,gxjp=5,jgsvq=3," +
            "pc-,xn=4,nsz=4,bzzb-,pt=2,rxjl=1,sx=1,vf=2,qfh-,ddzvp-,zn-,zqr-,fc-,zrd-,zpgc=5,smh-,lz=7,bg-,vh=9,rg=8,qglh=6,vk-,rfb-,fjrxpz=4,jlj=7," +
            "blqvx=2,kch-,hkh-,rjgg=2,dh-,bjmvp-,psqnq-,tvzr=8,fcgv=6,vtb-,bg-,zcn=6,xjr-,nts=1,gvlz-,bdx=1,qfh-,fzrjx=5,pfcldf=3,pppv=1,vfq-,rrh-," +
            "bhp=9,gp-,phnnf-,xj=1,dm-,hb-,bvm-,dhz=6,rb=7,tf-,llm=3,gc=5,fkm=6,gm=7,hk-,nbv-,bhml-,dlxn=9,mz=7,kd-,pb=7,qnfcn=3,kvz-,nm=5,jpzvb-,pg=5," +
            "rs-,pvt=2,trcm-,jkjhc=1,bmmz-,cz-,lz=9,bgq=5,fvtbs-,rbzz-,lgq=3,cfm-,llm=9,phnnf=6,fhnsv-,cfm-,qllcf=4,czl=4,ph-,kg-,dpf=4,sn=6,bhp=9," +
            "nbv=8,sn=4,rmxpmp=4,lrd-,lhb-,hf=4,txx-,prqfc=8,kg=1,lmb=7,pzzj=8,ddzvp=6,jk=9,zs-,lt=1,bjd-,rgq-,zqr-,xpq-,hf=9,mxth=4,vtv=1,xmkjhp=1," +
            "jl=1,xk=1,rkp=4,vn-,kvs-,mg=3,gkl=3,dpp=2,xmn-,hk-,mv=2,srt=8,xj-,tg=4,kg=2,dfx-,rknn=8,cr-,cmmbf-,xjm=7,cbf-,lznll-,hkh-,zbh=5,brgd-," +
            "dxzn=6,cpc=9,kvs=7,nm=1,sxzxpp=3,bv-,fbgmpx=2,gkr-,jjh=8,dl=7,bvm=5,td=1,bxm=7,krbj-,xm=8,jpp=1,mkf=7,cz-,vxkc=6,ns=8,bts=8,qnfcn-,mpn-," +
            "jjh=2,cr-,vmn=3,ghq=6,tbmrng=8,gkl-,qpvh=2,xfbj-,bjmvp=5,bts=7,bkmfj=8,bqzn-,jh=8,hms-,xjm-,slph=3,pb-,hjlm=8,chl-,zsb=8,sr=4,ll=5,tkkb=8," +
            "kk-,ns-,xc=3,qbfmk=8,sq=3,vtv-,sntr-,lg=3,jnpjz=5,bnk=2,zlfq-,gd-,bgq=9,rkp=5,dx-,vh=3,zrbzg=6,nlp-,blqvx-,ttd-,nsz-,rrh=2,cxh=2,dbtf=3," +
            "hlgrxq-,qgbcv-,pcv-,rdx-,vq-,zjl=5,qjz-,gxjp-,jz=6,bsh-,lgcz=7,vtz=9,tvzr-,kl=3,rmxpmp=4,gkr-,jn=6,czl-,ff=1,df=8,dq=7,rdmc=2,trcm-,gfsfrt-," +
            "npm-,rcqx=1,rvtsrh=2,gtv-,rtck-,rv=3,npm-,vg-,px-,vdzj=7,qpt-,tf-,fn=2,dd-,hj-,rv=9,rzx-,dxv-,drkvx=8,sqnn-,vg=2,pt=2,rdppqb=1,dl=4,sx=9," +
            "dxv=1,bxnljc-,dm=7,dxkjz=6,xp-,bjq-,dpf-,ggls=6,pjd-,zdp-,sr-,gzxqx=9,qxd=3,qjhc-,cm-,kf-,gq=2,nbj=9,vdzj-,tg=1,cv-,gvlz-,cc=6,vr-,bzzb=3," +
            "mx-,tt=2,pgm-,szrt=4,dq=8,rkp-,xpq-,gv-,fsp-,njf-,qsrqxc-,gxxlhv=9,ktj=8,td-,bv=6,jrvx-,xj=8,rv=6,xd=2,rfb-,ld-,qqdv=7,qs=4,qsrqxc=7,hg=2," +
            "kz=1,dh-,bj-,fkd=1,bng=6,mh=8,rd=9,kn=4,nts-,fbc=5,jjh=2,hc-,ld-,cssc-,cksg=3,cv=6,bts-,lq-,dkhtfp=1,bjq=6,sqnn=5,fbg-,npm=1,dlxn-,rd-," +
            "rhh=3,gd=3,mhd-,fxb-,smcrjx-,vv=3,hd=7,jlj=3,jn-,td-,sq-,smh=7,qs-,krbj-,xd=5,lq-,cmvp-,zqr=6,zlx=1,pt=8,jpp-,ccqn-,bqzn-,xbb-,lt=1,bnpj-," +
            "qbmrv-,prqfc=3,zz=2,nsz-,xjm=8,dpf=6,bppl-,qglh=3,ptrr=9,qbmrv-,vs=6,rdppqb=6,fxb-,tzndx-,lgcz-,rbzz-,zl-,xjm=9,fxb=6,pzzj-,rg=9,tn=9," +
            "mdn-,hf=7,vtz-,mv-,rhh-,dmmpl-,mtqr-,nkzqc=3,brsm=4,hj-,jrxhs-,hc=2,tbmrng=3,qp=8,mggjl-,bg=3,dfblmt-,ll=1,nr-,tt-,pj-,kd=3,fcgv=9,td=7," +
            "sqnn-,ckkl-,lqjl-,bj=2,mggjx-,skv=8,phpp-,sntr=2,bjq-,txk=8,zp=6,rht-,rcqx-,blsgt=5,tn=9,hz=5,tbmrng=1,plk-,vfq-,hl-,qhmj=5,tnv-,mggjx-," +
            "rzx=4,rskn=8,cv-,zrbzg=3,gd-,cv=3,xj=3,tcrtbl=6,fkm=6,ddzvp=5,fxng=7,fzfll-,gbfl-,nm-,vbb=6,rgh-,qv-,hb=5,vhmc-,hjq=2,pgtvq=3,lqk-,cpc-," +
            "vkh-,cksg-,hb-,fnl-,ph-,bg=3,smd=6,sqnn-,hms=8,mj-,cfm-,rtck-,kf=5,rfb=9,vkvh=4,rjgg-,ktj=2,mggjx-,bjq=7,vkvh-,qhj-,sk-,slph=3,nkzqc-,vr=1," +
            "cv-,btvg-,ht-,skv-,qqdv-,gm=5,vs-,nkzqc-,rskn=9,gkl-,cc=4,jz=8,fdv=4,fcgv=1,rkp-,sntr=3,zl=2,btvg=3,rz-,tbtq-,tqn=6,rd=5,fz-,fnqs-,gfsfrt-," +
            "nts=9,nt=1,zdp=3,rmx-,qqdk-,bng=3,lg=8,qllcf=9,kf=9,rfq=2,lft=5,pg=4,zz-,dxzn-,czcp-,hb-,lln-,plgr-,bhp=3,bxkk=4,jgsvq=6,ph-,sntr-,cpp=4," +
            "dhz=2,dxzn=7,bxnljc=6,ggls-,zlch-,dkv-,bts-,ckkl-,jpp-,qjz-,lzbs-,mpn-,vfq-,jlj-,np-,nmc-,ds-,mkf=6,ht-,pd-,cxh-,jpp=7,lln=4,jk=9,fc-,dhz-," +
            "cpj=1,qs=1,dp-,nhhv=9,ccqn-,hjlm-,lrz-,cbfq=2,xdjxq=6,ccqn-,lch-,llj-,vhmc-,zl-,jz=4,qbq-,tbmrng-,lg-,xpprc-,xhlh=7,fzrjx=7,zx-,zdvmh-," +
            "vrd-,jpp=2,mfbx=1,rg-,bts=5,nkzqc=3,szrt-,df=7,hj-,xnph-,fq-,xj-,rdmc-,sn=4,zs-,cpp=1,txx-,zlch-,bhp=3,zgf-,nxf=4,tpnt=2,fgz-,xfbj=5,slph=9," +
            "fgz=7,qdb-,szrt-,rtbrrv=3,qpvh=4,smcrjx=8,qbfmk=7,clkv=4,smh-,dfx-,xpq-,pt-,lqjl-,mds-,cjt-,fc-,qfh=9,sqnn=1";
    }
}