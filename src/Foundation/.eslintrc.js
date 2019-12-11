module.exports = {
    "root": true,
    "env": {
        "browser": true,
        "commonjs": true,
        "es6": true,
        "jquery": true
    },
    "parser": "babel-eslint",
    "plugins": [
        "@episerver/cms",
    ],
    "globals": {
        "notification": "readonly",
        "window": "writable",
        "convertFormData": "writable",
        "response": "readonly",
        "axios": "readonly",
        "feather": "readonly"
    },
    "extends": [
        "eslint:recommended",
        "plugin:@episerver/cms/recommended",
    ]
};
