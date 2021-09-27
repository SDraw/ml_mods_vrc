
####### Expanded from @PACKAGE_INIT@ by configure_package_config_file() #######
####### Any changes to this file will be overwritten by the next CMake run ####
####### The input file was leapsdk-config.cmake.in                            ########

get_filename_component(PACKAGE_PREFIX_DIR "${CMAKE_CURRENT_LIST_DIR}/../../../" ABSOLUTE)

####################################################################################

if(IS_DIRECTORY ${PACKAGE_PREFIX_DIR}/lib/x64 OR
    IS_DIRECTORY ${PACKAGE_PREFIX_DIR}/lib/x86)
  if(CMAKE_SIZEOF_VOID_P EQUAL 8)
    set(_arch_folder x64)
  else()
    set(_arch_folder x86)
  endif()
endif()

include(${PACKAGE_PREFIX_DIR}/lib/${_arch_folder}/cmake/LeapCTargets.cmake)
