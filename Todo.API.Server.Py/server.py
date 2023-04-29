import uuid
import todo
import todo_list

from flask import Flask, request, jsonify, abort

app = Flask(__name__)

TodoLists = {}
Todos = {}

# add some headers to allow cross origin access to the API on this server, necessary for using preview in Swagger Editor!
@app.after_request
def apply_cors_header(response):
    response.headers['Access-Control-Allow-Origin'] = '*'
    response.headers['Access-Control-Allow-Methods'] = 'GET,POST,DELETE'
    response.headers['Access-Control-Allow-Headers'] = 'Content-Type'
    return response

# 
# POST new list
# GET all lists
@app.route('/todo-list', methods=['POST', 'GET'])
def handle_base_list():
    if request.method == 'POST':
        new_list = request.get_json(force=True)
        print('Got new list to be added: {}'.format(new_list))
        new_list['id'] = uuid.uuid4()
        TodoLists.append(new_list)
        return jsonify(new_list), 200
    else:
        return jsonify(buildCompleteTodoLists()), 200

# define endpoint for getting and deleting existing todo lists
# GET get all entries
# DELETE remove list with all entries
@app.route('/todo-list/<listId>', methods=['GET', 'DELETE'])
def handle_list(listId):
    # find todo list depending on given list id
    list_item = None
    for l in TodoLists:
        if l['id'] == listId:
            list_item = l
            break
    # if the given list id is invalid, return status code 404
    if not list_item:
        abort(404)
    if request.method == 'GET':
        # find all todo entries for the todo list with the given id
        print('Returning todo list...')
        return jsonify([i for i in Todos if i['listId'] == listId])
    elif request.method == 'DELETE':
        # delete list with given id
        print('Deleting todo list...')
        removeTodoList(listId)
        return '', 200

#
# POST add entry to list
@app.route('/todo-list/<listId>/entry', methods=['POST'])
def handle_base_entry(listId):
    # find todo list depending on given list id
    list_item = None
    for l in TodoLists:
        if l['id'] == listId:
            list_item = l
            break
    # if the given list id is invalid, return status code 404
    if not list_item:
        abort(404)
    new_entry = request.get_json(force=True)
    new_entry['id'] = uuid.uuid4
    new_entry['listId'] = listId
    Todos.append(new_entry)
    return jsonify(new_entry), 200

#
# GET return an entry
# PUT update an entry
# DELETE remove an entry
@app.route('/todo-list/<listId>/entry/{entryId}', methods=['GET','PUT','DELETE'])
def handle_entry(listId, entryId):
    # find todo list depending on given list id
    list_item = None
    for l in TodoLists:
        if l['id'] == listId:
            list_item = l
            break
    # if the given list id is invalid, return status code 404
    if not list_item:
        abort(404)

    entry_item = None
    for e in Todos:
        if e['id'] == entryId and e['listId'] == listId:
            entry_item = e
            break
    
    if not entry_item:
        abort(404)
    
    if request.method == 'GET':
        return jsonify(entry_item), 200
    elif request.method == 'PUT':
        update_entry = request.get_json(force=True)

        Todos[entryId]['name'] = update_entry['name']
        Todos[entryId]['description'] = update_entry['description']

        return jsonify(Todos[entryId]), 200
    elif request.method == 'DELETE':
        Todos.remove(entryId)


# helper methods
def buildCompleteTodoLists():
    complete_list = []
    for list in TodoLists:
        complete_list.append(buildTodoList(list))
    return complete_list

def buildCompleteTodoList(id):
    list_entries = []
    for entry in Todos:
        if entry['listId'] == id:
            list_entries.append(entry)
    list = TodoLists[id]
    list['entries'] = list_entries
    return list

def buildTodoList(list):
    entries = []
    for entry in Todos:
        if entry['listId'] == list['id']:
            entries.append(entry)
    list['entries'] = entries
    return list

def removeTodoList(id):
    for t in Todos:
        if t['listId'] == id:
            Todos.remove(t)
    TodoLists.remove(TodoLists[id])

def addMockData():
    # create unique id for lists, entries
    todo_list_1_id = '1318d3d1-d979-47e1-a225-dab1751dbe75'
    todo_list_2_id = '3062dc25-6b80-4315-bb1d-a7c86b014c65'
    todo_list_3_id = '44b02e00-03bc-451d-8d01-0c67ea866fee'
    todo_1_id = uuid.uuid4()
    todo_2_id = uuid.uuid4()
    todo_3_id = uuid.uuid4()
    todo_4_id = uuid.uuid4()

    # define internal data structures with example data
    todo_lists = [
        {'id': todo_list_1_id, 'name': 'Einkaufsliste', 'entries': []},
        {'id': todo_list_2_id, 'name': 'Arbeit', 'entries': []},
        {'id': todo_list_3_id, 'name': 'Privat', 'entries': []},
    ]
    todos = [
        {'id': todo_1_id, 'listId': todo_list_1_id , 'name': 'Milch', 'description': '3x'},
        {'id': todo_2_id, 'listId': todo_list_1_id , 'name': 'Eier', 'description': '6x'},
        {'id': todo_3_id, 'listId': todo_list_2_id , 'name': 'Snackbox auffüllen', 'description': 'Mit geilem Scheiß'},
        {'id': todo_3_id, 'listId': todo_list_3_id , 'name': 'Sauber machen', 'description': 'Jetzt aber wirklich'},
    ]

    TodoLists.append(todo_lists)
    Todos.append(todos)

if __name__ == '__main__':
    app.debug = True
    app.run(host='0.0.0.0', port=8080)