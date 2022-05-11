const websocket = require('ws');
const wss = new websocket.Server({
    port: 3213
});


wss.on('connection', function(ws,req) {

    ws.defineUser = function(user){
        return user
    }

    ws.generateToken = function(){
        function getRandomInt( min, max ) {
            return Math.floor( Math.random() * ( max - min + 1 ) ) + min;
        }
        var tokens = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
        chars = 10,
        segments = 10,
        keyString = "";
        
        for( var i = 0; i < segments; i++ ) {
            var segment = "";
            
            for( var j = 0; j < chars; j++ ) {
                var k = getRandomInt( 0, 35 );
                segment += tokens[ k ];
            }
            
            keyString += segment;
            
            if( i < ( segments - 1 ) ) {
                keyString += "";
            }
        }
        
        return keyString;
    }
    ws.on('message', async function(message){
        messageObject = JSON.parse(message);
        if(!messageObject["username"] || !messageObject["action"]) return sendErrorMessage(ws,"(1)")
        if(messageObject["action"] === 'getToken'){
            if(!messageObject["password"]) return sendErrorMessage(ws,"(2)")


            ws.token = await ws.generateToken()
            ws.username = await ws.defineUser(messageObject["username"])


            console.log(ws.token, ws.username)
            let json = {
                succes: 'true',
                action: 'giveToken',
                token:ws.token
            }
            ws.send(JSON.stringify(json))
        } else if (messageObject["action"] === "sendMessage"){
            console.log(messageObject)
            if(!messageObject['token']) return sendErrorMessage(ws,"(5)")
            if(!messageObject["message"]) return sendErrorMessage(ws,"(6)")

            if(!messageObject["username"] === ws.username) return sendErrorMessage(ws,"(7)")
            if(!messageObject['token'] === ws.token) return sendErrorMessage(ws,"(8)")

            let json = {
                username:ws.username,
                message: messageObject["message"],
                action: 'sendMessage',
                
            }
            let cooltable = JSON.stringify(json)

            wss.clients.forEach(client => {
                client.send(cooltable)
            })
            
        }
    })

    ws.on('close', () =>{
        console.log('websocket closed')
    })

    ws.on('error', (error) =>{
        console.log(error)
    })
})

console.log( (new Date()) + " Server is listening on port " + 3213);





function sendErrorMessage(ws,errorCode){
    let json = {
        error: `invalid request ${errorCode}`,
        action: 'error',
        
    }
    ws.send(JSON.stringify(json))
}