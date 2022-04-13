# TODO

```js
/*
Keywords:

* JSON
* object
* tree
* treeify
* replace
* traverse
* transform

Zero dependency...
*/

class ObjectTreeNode {
    constructor(o, parent, reference) {
        this.parent = parent;
        this.reference = reference;
        this.isArray = Array.isArray(o);
        const isObject = o instanceof Object;
        if (!isObject) {
            this.value = o;
        }
        this.properties = isObject
            ? Object.entries(o).map(entry => {
                entry[1] = new ObjectTreeNode(entry[1], this, entry);
                return entry;
            })
            : [];
    }

    createEntry(name, value) {
        const entry = [name];
        entry.push(new ObjectTreeNode(value, this, entry));
        return entry;
    }

    updateName(entry, name) {
        if (entry) {
            entry[0] = name;
        }
    }

    updateValue(entry, value) {
        if (entry) {
            entry[1] = new ObjectTreeNode(value, this, entry);
        }
    }

    updateEntry(entry, name, value) {
        if (entry) {
            this.updateName(entry, name);
            this.updateValue(entry, value);
        }
    }

    // TODO: Return update count
    clearEmpty() {
        for (let node = this; node?.properties.length === 0; node = node.parent) {
            const npp = node.parent?.properties;
            if (!npp?.splice(npp?.indexOf(node.reference), 1)) {
                node.value = null;
            }
        }
    }

    // TODO: Return changes count!
    transform(f) {
        for (const entry of this.properties) {
            entry[1].transform(f);
        }
        f(this);
    }

    toObject() {
        return 'value' in this
            ? this.value
            : this.isArray
                ? this.properties.map(entry => entry[1].toObject())
                : Object.fromEntries(this.properties.map(entry => [entry[0], entry[1].toObject()]));
    }
}

class JsonTreeNode extends ObjectTreeNode {
    constructor(text) {
        super(JSON.parse(text), null, null);
    }

    toJSON(...args) {
        return JSON.stringify(this.toObject(), ...args);
    }
}

const test = `{
    "a": {
        "b": null,
        "c": "text1",
        "d": 0,
        "e": {
            "f": [
                null,
                "text2",
                1,
                {
                    "g": {
                        "h": 2
                    }
                }
            ]
        }
    }
}`;

const test2 = `{
    "x": {
        "y": [
            {
                "h": 2
            }
        ]
    },
    "z": 9
}`;

const test3 = `{
    "x": {
        "y": [
            {
                "h": 2
            }
        ]
    }
}`;

// Examples

const root = new JsonTreeNode(test3);

let count = 0;

// Rename
// TODO: Add count
count += root.transform(node => node.updateName(node.properties.find(entry => entry[0] === 'b'), 'bb'));

// Update value by name
// TODO: Add count
count += root.transform(node => node.updateValue(node.properties.find(entry => entry[0] === 'd'), 9));

// Update whole entry by value
// TODO: Add count
count += root.transform(node => node.updateEntry(node.properties.find(entry => entry[1].value === 'text1'), 'q', 'text4'));

// Add
// TODO: Add count
count += root.transform(node => {
    if (node.properties.some(entry => entry[0] === 'bb')) {
        node.properties.push(node.createEntry('y', [3, { z: 5 }]));
    }
});

// Remove
// TODO: Add count
count += root.transform(node => {
    const index = node.properties.findIndex(entry => entry[0] === 'h');
    if (index > -1) {
        node.properties.splice(index, 1);
        // optional
        node.clearEmpty();
    }
});

// Reverse
count += root.transform(node => node.isArray ? 0 : node.properties.reverse().length);

// TODO:
// Example for array name ignored, vllt.mit 
// if (this.isArray) {
// use https://github.com/MikeMcl/big.js/wiki compare
//    this.properties.sort((a, b) => a[0] - b[0]);
// }

console.log(JSON.stringify(root));
console.log();
console.log(root.toJSON(null, 2));
```
