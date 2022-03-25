const sass = require('node-sass');

module.exports = function (grunt) {
    'use strict';

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        // Sass
        sass: {
            options: {
                implementation: sass,
                sourceMap: true,
                outputStyle: 'compressed'
            },
            dist: {
                files: [
                    {
                        expand: true,
                        cwd: "Styles",
                        src: ["**/*.scss"],
                        dest: "wwwroot/css",
                        ext: ".css"
                    }
                ],
                options: {
                    banner: '/*\nConcatinated CS file \n' +
                        'Author: Tee Kaeophian \n' +
                        // 'Created Date: <%= grunt.template.today("yyyy-mm-dd") %>' +
                        '\n */ \n'
                },
            }
        },
        uglify: {
            js: {
                files: { 'wwwroot/js/site.js': 'Scripts/**/*.js' },
                options: {
                    preserveComments: false,
                    banner: '/*\nConcatinated JS file \n' +
                        'Author: Tee Kaeophian \n' +
                        // 'Created Date: <%= grunt.template.today("yyyy-mm-dd mm:ss") %>' +
                        '\n */ \n'
                }
            },
            vueJs: {
                files: grunt.file.expandMapping(['ClientApp/**/*.js'], 'wwwroot/app/', {
                    rename: function(destBase, destPath) {
                        return destBase+destPath.replace("ClientApp", '');
                    }
                }),
                options: {
                    preserveComments: false,
                    banner: '/*\nConcatinated VueJS file \n' +
                        'Author: Tee Kaeophian \n' +
                        // 'Created Date: <%= grunt.template.today("yyyy-mm-dd mm:ss") %>' +
                        '\n */ \n'
                }
            }
        },
        concat: {
            dist: {
                options: {
                    separator: '\n\r',
                    banner: '/*\nConcatinated JS file \n' +
                        'Author: Tee Kaeophian \n' +
                        'Created Date: <%= grunt.template.today("yyyy-mm-dd mm:ss") %>' +
                        '\n */ \n'
                },
                cwd: "Scripts",
                src: ["Scripts/**/*.js"],
                dest: 'wwwroot/js/site.js'
            }
        },
        watch: {
            css: {
                files: 'Styles/**/*.scss',
                tasks: ['sass'],
                options: {
                    livereload: true,
                },
            },
            scripts: {
                files: ["Scripts/**/*.js", 'ClientApp/**/*.js'],
                tasks: ["uglify"]
            },
        }
    });

    grunt.loadNpmTasks('grunt-contrib-concat');

    grunt.loadNpmTasks('grunt-contrib-uglify');

    grunt.loadNpmTasks('grunt-sass');

    grunt.loadNpmTasks('grunt-contrib-watch');

    grunt.registerTask('default', ['sass', 'uglify:js', 'uglify:vueJs']);
};