ChatSmileysResolverObj = function(imgPath) {
    var path = imgPath,
    smileys = [                        // begin    //end
        {img: "angel_smile.gif", reg: /(^|\s)o:-?\)(?=($|\s))/gm, alt: "angel" },
        { img: "devil_smile.gif", reg: /(^|\s)&gt;:-?\)(?=($|\s))/gm, alt: "devil" },// &gt; means >
        { img: "regular_smile.gif", reg: /(^|\s):-?\)(?=($|\s))/gm, alt: "smile" },
        { img: "broken_heart.gif", reg: /(^|\s)=\(\((?=($|\s))/gm, alt: "broken heart" },
        { img: "sad_smile.gif", reg: /(^|\s):-?\((?=($|\s))/gm, alt: "sad" },
        { img: "angry_smile.gif", reg: /(^|\s)x-?\((?=($|\s))/gm, alt: "angry" },
        { img: "confused_smile.gif", reg: /(^|\s):-?\/(?=($|\s))/gm, alt: "confused" },
        { img: "cry_smile.gif", reg: /(^|\s):-?\(\((?=($|\s))/gm, alt: "cry" },
        { img: "embaressed_smile.gif", reg: /(^|\s):-?\[(?=($|\s))/gm, alt: "embaressed" },
        { img: "heart.gif", reg: /(^|\s):-?x(?=($|\s))/gm, alt: "heart" },
        { img: "kiss.gif", reg: /(^|\s):-?\*(?=($|\s))/gm, alt: "kiss" },
        { img: "omg_smile.gif", reg: /(^|\s):-?o(?=($|\s))/gm, alt: "omg" },
        { img: "shades_smile.gif", reg: /(^|\s)8-?\)(?=($|\s))/gm, alt: "shades" },
        { img: "teeth_smile.gif", reg: /(^|\s):-?D(?=($|\s))/gm, alt: "big grin" },
        { img: "tounge_smile.gif", reg: /(^|\s):-?[pP](?=($|\s))/gm, alt: "tounge" },
        { img: "wink_smile.gif", reg: /(^|\s);-?\)(?=($|\s))/gm, alt: "wink" },
        { img: "kentico.gif", reg: /(^|\s)\(kentico\)(?=($|\s))/gm, alt: "kentico" },
        { img: "thumbs_up.gif", reg: /(^|\s)\(thumbsup\)(?=($|\s))/gm, alt: "thumbs-up" },
        { img: "thumbs_down.gif", reg: /(^|\s)\(thumbsdown\)(?=($|\s))/gm, alt: "thumbs-down" }
    ];


    this.ResolveSmileys = function(text) {
        for (var i = 0; i < smileys.length; i++) {
            var sm = smileys[i];
            text = text.replace(sm.reg, getImage(sm));
        }
        return text;
    };
    

    function getImage(smiley) {
        return "$1<img src=\"" + path + smiley.img + "\" alt=\"" + smiley.alt + "\" />$2";
    }
}