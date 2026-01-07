let counter = 0;
let counter2 = 1;

document.addEventListener('DOMContentLoaded', load);

function load() {
    const start = counter;
    const end = start + 10;
    counter = end;

    fetch(`/followingposts?start=${start}&end=${end}`)
    .then(response => response.json())
    .then(data => {
        console.log(data);
        if(!data.too_large && counter2 !== 1) {
            let num = counter2 - 1;
            document.querySelector('#loader' + num).style.display = 'none'
        }
        for(let i = 0; i < data.texts.length; i++) {
            add_post(data.texts[i], data.dates[i], data.posters[i], data.likes[i], data.ids[i]);
        }
        if(data.too_large) {
            const button = document.createElement('div');
            button.id = 'loader' + counter2;
            counter2++;
            button.style.display = 'block';
            button.style.textAlign = 'right';
            button.style.padding = '15px';
            button.innerHTML = '<button id="load_posts">Load Posts</button>';
            document.querySelector('#posts').append(button);
            document.querySelector('#load_posts').addEventListener('click', function() {
                load();
            });
        }
    })
}

function add_post(text, date, poster, likes, id) {
    const post = document.createElement('div');
    post.style.border = '2px solid gray';
    post.style.borderRadius = '8px';
    post.style.margin = '50px';
    post.style.marginLeft = '400px';
    post.style.marginRight = '400px';
    post.style.padding = '10px';

    post.innerHTML = '<button class="' + poster + '" style="background: none; border: none; padding-left: 0px"><h5>' + poster + '</h5></button>';
    post.innerHTML += '<br/>';
    post.innerHTML += text + '<br/>';

    let time = date.substring(11, date.length - 5);
    date = date.substring(0, 10);
    post.innerHTML += '<div style="color: gray">' + date + ' at ' + time;

    post.innerHTML += '<button id="likes' + id + '" class="button" style="margin-top: 5px; padding-top: 3px; padding-bottom: 3px;">Likes: ' + likes + '</button>';

    document.querySelector('#posts').append(post);

    document.querySelector('.' + poster).addEventListener('click', function() {
        window.location.href = '/' + poster;
    });

    document.querySelector('#likes' + id).addEventListener('click', function() {
        like(id)
    });
}

function like(id){
    fetch(`/like?postId=` + id)
    .then(response => response.json())
    .then(data => {
        console.log(data);
        document.querySelector('#likes' + id).innerHTML = 'Likes: ' + data.likes;
    })
}