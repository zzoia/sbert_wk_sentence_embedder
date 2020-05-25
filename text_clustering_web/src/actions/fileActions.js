const txt = "text/plain";
const json = "application/json";

export const getNumberOfTexts = (file) => {
    return new Promise(getTextFunc(file))
        .then(getLinesFunc(file.type), noop)
        .then(getLenFunc, noop);
};

export const getTexts = (file) => {
    return new Promise(getTextFunc(file))
        .then(getLinesFunc(file.type), noop);
}

const noop = data => data;

const getLinesFunc = type => text => {

    if (type === txt) {
        return text.split(/\r?\n/);
    }

    if (type === json) {
        return JSON.parse(text);
    }

    return [];
}

const getLenFunc = lines => lines.length;

const getTextFunc = file => (resolve, reject) => {

    if (file.type === txt || file.type === json) {

        const reader = new FileReader();
        reader.onload = async (event) => {
            const text = (event.target.result);
            resolve(text);
        };

        reader.readAsText(file);

    } else {
        reject({ type: file.type });
    }

};