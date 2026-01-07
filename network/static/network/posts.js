let start = 0;
document.addEventListener('DOMContentLoaded', load);

function load() {
    if (window.location.search !== "") {
        const params = new URLSearchParams(window.location.search);
        start = params.get('start');
    }

    let end = +start + 10;

    fetch(`/posts?start=${start}&end=${end}`)
    .then(response => response.json())
    .then(data => {
        console.log(data)

        for(let i = 0; i < data.texts.length; i++) {
            add_post(data.texts[i], data.dates[i], data.posters[i], data.likes[i], data.ids[i], data.user);
        }

        const buttons = document.createElement('div');
        buttons.id = 'buttons';
        if(start > 0) {
            buttons.style.display = 'flex';
            buttons.style.justifyContent = 'space-between';
        }
        document.querySelector('#posts').append(buttons);

        if(start > 0){
            const button = document.createElement('div');
            button.style.padding = '15px';
            button.innerHTML = '<button id="prev_load">Previous Posts</button>';
            document.querySelector('#buttons').append(button);

            document.querySelector('#prev_load').addEventListener('click', function() {
                if (start >= 10) {
                    window.location.href = '/' + '?start=' + (+start - 10);
                }
                else {
                    window.location.href = '/' + '?start=0';
                }
            });
        }

        if(data.too_large) {
            const button = document.createElement('div');
            button.style.textAlign = 'right';
            button.style.padding = '15px';
            button.innerHTML = '<button id="load_posts">Load Posts</button>';
            document.querySelector('#buttons').append(button);
            document.querySelector('#load_posts').addEventListener('click', function() {
                window.location.href = '/' + '?start=' + end;
            });
        }
    })
}

function add_post(text, date, poster, likes, id, user) {
    const post = document.createElement('div');
    post.style.border = '2px solid gray';
    post.style.borderRadius = '8px';
    post.style.margin = '50px';
    post.style.marginLeft = '400px';
    post.style.marginRight = '400px';
    post.style.padding = '10px';
    post.id = 'post' + id;

    post.innerHTML = '<button class="' + poster + '" style="background: none; border: none; padding-left: 0px"><h5>' + poster + '</h5></button>';
    post.innerHTML += '<br>';

    if (poster === user) {
        post.innerHTML += '<button id="edit' + id + '" style="border: none; background: none; color: #007bff; padding-left: 0px">Edit</button><br>';
    }

    post.innerHTML += text + '<br>';

    let time = date.substring(11, date.length - 5);
    date = date.substring(0, 10);
    post.innerHTML += '<div style="color: gray">' + date + ' at ' + time + '</div>';


    post.innerHTML += '<button id="likes' + id + '" class="button" style="margin-top: 5px; padding-top: 3px; padding-bottom: 3px;">Likes: ' + likes + '</button>';

    document.querySelector('#posts').append(post);

    document.querySelector('.' + poster).addEventListener('click', function() {
        window.location.href = '/' + poster;
    });

    document.querySelector('#likes' + id).addEventListener('click', function() {
        like(id);
    });

    if (poster === user){
        document.querySelector('#edit' + id).addEventListener('click', function() {
            edit(id, poster, text, date, likes, user);
        });
    }
}

function like(id){
    fetch(`/like?postId=` + id)
    .then(response => response.json())
    .then(data => {
        console.log(data);
        document.querySelector('#likes' + id).innerHTML = 'Likes: ' + data.likes;
    })
}

function edit(id, poster, text, date, likes, user) {
    let editarea = document.querySelector('#post' + id);

    editarea.innerHTML = '<button class="' + poster + '" style="background: none; border: none; padding-left: 0px"><h5>' + poster + '</h5></button>';
    editarea.innerHTML += '<br>';

    editarea.innerHTML += '<textarea style="width: 1000px" id="textfield' + id + '"></textarea>';
    editarea.innerHTML += '<br>';
    editarea.innerHTML += '<button id="submit' + id + '" class="button"> Submit </button>';

    document.querySelector('#post' + id).innerHTML = editarea.innerHTML;

    document.querySelector('#submit' + id).addEventListener('click', function() {
        let text = document.querySelector('#textfield' + id).value;

        fetch('/edit?postId=' + id +'&start=' + start + '&text=' + text)
        .then(response => response.json())
        .then(data => {
            console.log(data);
            let post = document.querySelector('#post' + id);
            post.innerHTML = '<button class="' + poster + '" style="background: none; border: none; padding-left: 0px"><h5>' + poster + '</h5></button>';
            post.innerHTML += '<br>';

            if (poster === user) {
                post.innerHTML += '<button id="edit' + id + '" style="border: none; background: none; color: #007bff; padding-left: 0px">Edit</button><br>';
            }

            post.innerHTML += text + '<br>';

            let time = date.substring(11, date.length - 5);
            date = date.substring(0, 10);
            post.innerHTML += '<div style="color: gray">' + date + ' at ' + time + '</div>';

            post.innerHTML += '<button id="likes' + id + '" class="button" style="margin-top: 5px; padding-top: 3px; padding-bottom: 3px;">Likes: ' + likes + '</button>';



            document.querySelector('#post' + id).innerHTML = post.innerHTML;

            document.querySelector('.' + poster).addEventListener('click', function() {
                window.location.href = '/' + poster;
            });

            document.querySelector('#likes' + id).addEventListener('click', function() {
                like(id);
            });

            if (poster === user){
                document.querySelector('#edit' + id).addEventListener('click', function() {
                    edit(id, poster, text, date, likes, user);
                });
            }
        })
    });
}