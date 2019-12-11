/// <binding AfterBuild='default' />
let gulp = require('gulp'),
    sass = require('gulp-sass'),
    rename = require('gulp-rename'),
    changed = require('gulp-changed'),
    autoprefixer = require('gulp-autoprefixer'),
    sourcemaps = require('gulp-sourcemaps'),
    js_concat = require('gulp-concat'),
    fs = require('fs'),
    eslint = require('gulp-eslint');

gulp.task('sass', () => {
    return gulp.src('./assets/scss/main.scss')
        .pipe(sourcemaps.init())
        .pipe(changed('./assets/scss/', { extension: '.css' }))
        .pipe(sass({ outputStyle: 'compressed' }))
        .pipe(autoprefixer(['last 3 versions', '> 1%'], { cascade: true }))
        .pipe(rename({ suffix: '.min' }))
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('./assets/scss'));
});

gulp.task('js', () => {
    return gulp.src(['./assets/js/features/*.js', './assets/js/plugins/notify.js'])
        .pipe(sourcemaps.init())
        .pipe(js_concat('main.min.js'))
        //.pipe(js_minify())
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('./assets/js/'));
});

gulp.task('lint', gulp.series('js', () => {
    return gulp.src(['./assets/js/main.min.js', './clientresources/scripts/**/*.js', './modules/_protected/**/*.js'])
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.format('./node_modules/eslint-formatter-markdown/markdown.js', fs.createWriteStream('../../eslint-html-result.md')));
        //.pipe(eslint.failAfterError());
}));

gulp.task('watch', () => {
    gulp.watch('./assets/scss/**/*.scss', gulp.series('sass'));
    gulp.watch('./assets/js/features/**/*.js', gulp.series('js'));
});

gulp.task('default', gulp.parallel('sass', 'lint'));