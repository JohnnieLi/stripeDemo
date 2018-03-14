var express = require('express');
var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);
//app.use(express.static());
// app.use(express.static(path.join(__dirname, 'public')));
var bodyParser = require('body-parser')
app.use(bodyParser.urlencoded({
    extended: true
}));

app.use(function(req, res, next) {
  res.header("Access-Control-Allow-Origin", "*");
  res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
  next();
});


app.post('/payment', function(request, res){

 console.log(request.body);
 // Token is created using Checkout or Elements!
// Get the payment token ID submitted by the form:
var token = request.body.stripeToken; // Using Express
// Charge the user's card:
stripe.charges.create({
  amount: 999,
  currency: "usd",
  description: "Example charge",
  source: token,
}, function(err, charge) {
 if(err){
   return res.send(err);
 }else{
    return res.send(charge);
 }
});


});


// Set your secret key: remember to change this to your live secret key in production
// See your keys here: https://dashboard.stripe.com/account/apikeys
var stripe = require("stripe")("sk_test_BQokikJOvBiI2HlWgH4olfQ2");



app.get('/', function(req, res){
  //res.send('<h1>Hello world</h1>');
  res.sendFile(__dirname + '/index.html');
});


//io.on('connection', function(socket){
//  console.log('a user connected');
//   socket.on('disconnect', function(){
//    console.log('user disconnected');
//  });
//});
//uodhIKPxz0wBw1XZ8Tah
io.on('connection', function(socket){
  
    console.log('user connection');
    
    
  socket.on('send-nickname', function(nickname) {
    
    socket.nickname = nickname;
    console.log(socket.nickname);
           
    //using socket, just sent to one user who trigger this event
    socket.emit('chat message','Hello, welcome', socket.nickname);
  
    //using io, sent to ALL users including who trigger this event
    io.emit('chat message','New guy comes in!', socket.nickname);
  
    //using socket.broadcast,  sent to ALL users except for a certain socket.
    socket.broadcast.emit('chat message','Hi, I am new guy here.', socket.nickname);  
      
   });
     
    
  
  socket.on('chat message', function(msg, username){
    console.log(username);
    io.emit('chat message', msg, username);
    console.log('message: ' + msg);
  });
    
    
   socket.on('typing', function(username){
    socket.broadcast.emit('typing', username);
  });
    
    
  socket.on('disconnect', function(){
    console.log('user disconnected');
    socket.broadcast.emit('leave');
  });
    
    
});


http.listen(3000, function(){
  console.log('listening on *:3000');
});
    