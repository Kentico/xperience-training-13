// JScript File

//2D array of divs in sliders
var slidernodes = new Object();
//Array of curent pages on slider
var curentslide = new Object();
//Array of old pages on slider
var oldslide = new Object();
//Array of fadeIn/Out effects
var fadeFx = new Object();
//State of slider (0=fadeout, 1=fadein, 2=break)
var state = new Object();

var ContentSlider = new Class({
    // inicialization of slider
        initialize: function(id,fadeInTime,fadeOutTime,breakTime){
        this.id = id;
        this.fadeInTime = fadeInTime;
        this.fadeOutTime = fadeOutTime;
        this.breakTime = breakTime;
        slidernodes[this.id]=[];
        fadeFx[this.id]=[];
        curentslide[this.id]=0;
        oldslide[this.id]=0;
        var slider = $(this.id);
        var alldivs = $$("div.ContentPage");
        for (var i=0; i<alldivs.length; i++){
            if (alldivs[i].parentNode.id == this.id) {
                slidernodes[this.id].push(alldivs[i]);
                fadeFx[this.id].push(new Fx.Morph(alldivs[i],{duration:this.fadeInTime, wait:false}));
            }
        }
        if (slidernodes[this.id].length != 0){
            this.show(curentslide[this.id]);
        }
    },
    //show start page and hide others
    show:function(page){  
        slidernodes[this.id].each(function(item, index){
            item.setOpacity(0);
            item.setStyle('z-index','-10');
        });
        slidernodes[this.id][page].setOpacity(1);
        slidernodes[this.id][page].setStyle('z-index','10');
    },
    //turn page (fadeIn-fadeOut effect)
    turnPage:function(page,isAuto){
        if (isAuto) 
        {
            var tmp = this.id;
            state[tmp] = 0;
            fadeFx[this.id][curentslide[this.id]].start({'opacity': 0.0,'z-index': -10}).chain(function(){
                state[tmp] = 1;
                fadeFx[tmp][page].start({'opacity': 1.0,'z-index': 10}).chain(function(){
                    state[tmp] = 2;
                });
            });
            oldslide[this.id] = curentslide[this.id];
		    curentslide[this.id] = page;
        }
        else 
        {   //disable autoTurnPage
            if (typeof window[this.id+"timer"]!="undefined"){
		        clearTimeout(window[this.id+"timer"]);
            }
		    
            var tmp = this.id;
            if (state[tmp] == 0) {  //if fadeOut
                //Clear old chain
                fadeFx[this.id][oldslide[this.id]].clearChain();
                
                state[tmp] = 0;
                fadeFx[this.id][oldslide[this.id]].start({'opacity': 0.0,'z-index': -10}).chain(function(){
                    state[tmp] = 1;
                    fadeFx[tmp][page].start({'opacity': 1.0,'z-index': 10}).chain(function(){
                        state[tmp] = 2;
                    });
                });
                
                oldslide[this.id] = curentslide[this.id];
		        curentslide[this.id] = page;
            }
            else {  //if fadeIn or break
                //Clear old chain
                fadeFx[this.id][curentslide[this.id]].clearChain();
                
                state[tmp] = 0;
                fadeFx[this.id][curentslide[this.id]].start({'opacity': 0.0,'z-index': -10}).chain(function(){
                    state[tmp] = 1;
                    fadeFx[tmp][page].start({'opacity': 1.0,'z-index': 10}).chain(function(){
                        state[tmp] = 2;
                    });
                });
                
                oldslide[this.id] = curentslide[this.id];
		        curentslide[this.id] = page;
            }
        }
    }
});

// Auto turn page 
function autoTurnPage(slider,page,time,turn){
    try
    {
        if (page >= slidernodes[slider.id].length)
        {   
            page = 0;
        }

        if(turn)
        {
            slider.turnPage(page,true);
        }
    
        window[slider.id+"timer"]=setTimeout(function(){autoTurnPage(slider,page+1,time,true); },time);
    }
    catch (ex)
    {
    }
}