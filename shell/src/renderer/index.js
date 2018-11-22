var {ipcRenderer, remote} = require('electron');  



document.querySelector("#app").innerHTML = `
    <div style="position:absolute; top:0px; left:0px; right:0px; height:30px; background:lightgray">
        <table style="
        width: 100%;
        height: 100%;
        text-align: center;
        font-family:  sans-serif;
        ">
            <tr>
                <td><a href="javascript:load('http://cs.mindong.asia')">众包首页</a></td>
                <td><a href="javascript:loadO('http://cs.mindong.asia/help/index.html')">帮助</a></td>
                <td><a href="javascript:loadO('http://zisea.com/zslf.htm')">字海</a></td>
                <td><a href="javascript:loadO('http://zhs.glyphwiki.org/wiki')">glyphWiki</a></td>
            </tr>
        </table>
    </div>
    <div style="position:absolute;top: 30px;bottom: 0px;left: 0px;right: 0px;"> 
        <webview style="
        height: 100%;
        width: 100%;
        border: 0;
    " id="innerFrame" src="http://cs.mindong.asia" />
    </div>
    <div id="overlay" style="display: none; flex-flow:column; position:absolute;top: 30px;bottom: 100px;left: 20px;right: 20px;border: 3px solid gray;box-shadow: 0px 10px 11px #cccccc;">
        <div style="background:lightgray; text-align:right; padding-right:10px; flex: 0 1 auto;">
        <a href="javascript:hide()">关闭</a>
        </div> 
        <webview style="
            flex:1 1 auto;            
        " id="innerFrame2" src="about:blank" />
    </div>
`

document.body.style = "overflow-y:hidden;"

window.load = function(url) {
    //ipcRenderer.send('loadUrl', url);
    document.querySelector("#innerFrame").src = url;
}

window.loadO = function(url) {
    //ipcRenderer.send('loadUrl', url);
    document.querySelector("#innerFrame2").src = url;
    document.querySelector("#overlay").style.display="flex";
}


window.hide = function() {
    document.querySelector("#overlay").style.display="none";
}